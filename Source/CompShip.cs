using RimWorld;
using RimWorld.Planet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TraderShips.Quest;
using UnityEngine;
using Verse;
using Verse.AI;
using Verse.Sound;

namespace TraderShips
{

    [StaticConstructorOnStartup]
    class CompShip : ThingComp
    {
        public CompPropertiesShip Props => props as CompPropertiesShip;
        public LandedShip tradeShip;
        public bool mustCrash = false;
        public QuestPart_ShipsTradeRequest tradeRequest;

        ShipSprite sprite;

        public ShipSprite Sprite
        {
            get
            {
                if (sprite == null)
                {
                    sprite = Props.shipgen.CreateSprite();
                }

                return sprite;
            }
            set
            {
                sprite = value;
            }
        }

        public override void PostExposeData()
        {
            Scribe_Deep.Look(ref sprite, "shipSprite");
            Scribe_Deep.Look(ref tradeShip, "ship");
            Scribe_Values.Look(ref mustCrash, "mustCrash");
            Scribe_Deep.Look(ref tradeRequest, "tradeRequest");
        }

        public override void PostDraw()
        {
            if (mustCrash && parent.Spawned) return;

            Sprite.Draw(parent.DrawPos);
        }

        public override void CompTick()
        {
            if (tradeShip.Departed)
            {
                if (parent.Spawned) SendAway();
            }
            else
            {
                tradeShip.PassingShipTick();
            }

        }

        public override string CompInspectStringExtra()
        {
            string res =
                tradeShip.def.LabelCap + "\n" +
                "TraderShipsLeavingIn".Translate(GenDate.ToStringTicksToPeriod(tradeShip.ticksUntilDeparture));

            if (tradeRequest != null)
            {
                res += "\n" + "TraderShipsQuestRequestInfo".Translate(TradeRequestUtility.RequestedThingLabel(tradeRequest.requestedThingDef, tradeRequest.requestedCount).CapitalizeFirst(), (tradeRequest.requestedThingDef.GetStatValueAbstract(StatDefOf.MarketValue, null) * (float)tradeRequest.requestedCount).ToStringMoney(null));
            }

            return res;
        }


        public override void PostSpawnSetup(bool respawningAfterLoad)
        {
            if (respawningAfterLoad) return;
            if (mustCrash) return;

            if (Props.soundThud != null)
                Props.soundThud.PlayOneShot(parent);

            TaggedString info = "TraderShipsArrival".Translate(tradeShip.name, tradeShip.def.label, (tradeShip.Faction == null) ? "TraderArrivalNoFaction".Translate() : "TraderArrivalFromFaction".Translate(tradeShip.Faction.Named("FACTION")));
            Find.LetterStack.ReceiveLetter(tradeShip.def.LabelCap, info, LetterDefOf.PositiveEvent, parent, tradeShip.Faction);
        }

        public override string TransformLabel(string label)
        {
            return tradeShip.name;
        }

        public override void PostPostApplyDamage(DamageInfo dinfo, float totalDamageDealt)
        {
            bool leaving = false;

            if (totalDamageDealt > 25)
            {
                leaving = true;
            }
            else if (parent.HitPoints <= parent.MaxHitPoints * 0.9)
            {
                leaving = true;
            }

            if (leaving)
            {
                Messages.Message("TraderShipsLeavingDueToDamage".Translate(tradeShip.name), parent, MessageTypeDefOf.NegativeEvent, true);
                SendAway();
            }
        }

        public override IEnumerable<FloatMenuOption> CompFloatMenuOptions(Pawn negotiator)
        {
            string label = "TradeWith".Translate(tradeShip.GetCallLabel());

            yield return FloatMenuUtility.DecoratePrioritizedTask(new FloatMenuOption(label, delegate ()
            {
                Job job = JobMaker.MakeJob(Globals.TraderShipsTradeWithShip, parent);
                negotiator.jobs.TryTakeOrderedJob(job, JobTag.Misc);
            }, MenuOptionPriority.InitiateSocial, null, null, 0f, null, null), negotiator, parent);

            yield break;
        }

        Faction GetFaction(TraderKindDef trader)
        {
            if (trader.faction == null) return null;
            return (from f in Find.FactionManager.AllFactions where f.def == trader.faction select f).RandomElementWithFallback();
        }

        public void GenerateInternalTradeShip(Map map, TraderKindDef traderKindDef = null)
        {
            if (traderKindDef == null) traderKindDef = DefDatabase<TraderKindDef>.AllDefs.RandomElementByWeightWithFallback(x => x.CalculatedCommonality);

            tradeShip = new LandedShip(map, traderKindDef, GetFaction(traderKindDef));
            tradeShip.passingShipManager = map.passingShipManager;
            tradeShip.GenerateThings();
        }


        private void FulfillTradeRequest()
        {
            tradeRequest.FulfillRequest(tradeShip);

            QuestUtility.SendQuestTargetSignals(parent.questTags, "TradeRequestFulfilled", parent.Named("SUBJECT"));
            tradeRequest = null;
        }

        public override IEnumerable<Gizmo> CompGetGizmosExtra()
        {
            yield return new Command_Action
            {
                defaultLabel = "TraderShipsSendAway".Translate(),
                defaultDesc = "TraderShipsSendAwayDesc".Translate(),
                action = new Action(SendAway),
                icon = SendAwayTexture,
            };

            if (tradeRequest != null)
            {
                Command_Action command_Action = new Command_Action();
                command_Action.defaultLabel = "CommandFulfillTradeOffer".Translate();
                command_Action.defaultDesc = "CommandFulfillTradeOfferDesc".Translate();
                command_Action.icon = TradeCommandTex;
                command_Action.action = delegate ()
                {
                    if (tradeRequest == null)
                    {
                        Log.Error("Attempted to fulfill an unavailable request");
                        return;
                    }

                    WindowStack windowStack = Find.WindowStack;
                    TaggedString text = "CommandFulfillTradeOfferConfirm".Translate(GenLabel.ThingLabel(tradeRequest.requestedThingDef, null, tradeRequest.requestedCount));

                    windowStack.Add(Dialog_MessageBox.CreateConfirmation(text, delegate () { FulfillTradeRequest(); }, false, null));
                };

                if (!tradeRequest.CanFulfillRequest(tradeShip))
                {
                    command_Action.Disable("CommandFulfillTradeOfferFailInsufficient".Translate(TradeRequestUtility.RequestedThingLabel(tradeRequest.requestedThingDef, tradeRequest.requestedCount)));
                }

                yield return command_Action;
            }

            if (Prefs.DevMode)
            {
                yield return new Command_Action
                {
                    defaultLabel = "Dev: randomize sprite",
                    action = delegate ()
                    {
                        Color color = sprite.color;
                        sprite = Props.shipgen.CreateSprite();
                        sprite.color = color;
                        sprite.FinishCreating();
                    },
                };
                yield return new Command_Action
                {
                    defaultLabel = "Dev: randomize color",
                    action = delegate ()
                    {
                        sprite.color = ShipSprite.RandomColor();
                        sprite.FinishCreating();
                    },
                };
            }

            yield break;
        }

        private void SendAway()
        {
            if (!parent.Spawned)
            {
                Log.Error("Tried to send " + parent + " away, but it's unspawned.");
                return;
            }

            Map map = parent.Map;
            IntVec3 position = parent.Position;

            parent.DeSpawn();

            Skyfaller skyfaller = ThingMaker.MakeThing(Props.takeoffAnimation, null) as Skyfaller;
            if (!skyfaller.innerContainer.TryAdd(parent, true))
            {
                Log.Error("Could not add " + parent.ToStringSafe<Thing>() + " to a skyfaller.");
                parent.Destroy(DestroyMode.QuestLogic);
            }

            GenSpawn.Spawn(skyfaller, position, map, WipeMode.Vanish);
        }

        IntVec3 RandomNearby(IntVec3 loc)
        {
            int radius = Props.crashScatterRadius;
            return new IntVec3(loc.x + Rand.Range(-radius, radius), loc.y, loc.z + Rand.Range(-radius, radius));
        }

        public void Crash()
        {
            if (!mustCrash || !parent.Spawned) return;

            mustCrash = false;
            IntVec3 loc = parent.Position;
            Map map = parent.Map;
            if (TraderShips.settings.enableShipComponentDrops)
            {
                parent.Destroy(DestroyMode.Vanish);
            }
            else
            {
                parent.Destroy(DestroyMode.KillFinalize);
            }
            

            if (Props.crashScatterChunk != null)
            {
                for (int i = 0; i < Props.crashScatterChunkCount.RandomInRange; i++)
                {
                    GenSpawn.Spawn(Props.crashScatterChunk, RandomNearby(loc), map);
                }
            }
            
            if (Props.crashPilotKind != null)
            {
                for (int i = 0; i < Props.crashPilotCount.RandomInRange; i++)
                {
                    Pawn pawn = PawnGenerator.GeneratePawn(new PawnGenerationRequest(Props.crashPilotKind, tradeShip.Faction));
                    GenPlace.TryPlaceThing(pawn, RandomNearby(loc), map, ThingPlaceMode.Near);
                    pawn.TakeDamage(new DamageInfo(DamageDefOf.Bomb, 250, 0, -1, parent));
                    if (!pawn.Dead) HealthUtility.DamageUntilDowned(pawn);
                }
            }
            
            foreach (Thing thing in tradeShip.Goods)
            {
                thing.stackCount=(int)(thing.stackCount * TraderShips.settings.lootPercent/100);
                if(thing.stackCount > 0)
                {
                    GenPlace.TryPlaceThing(thing, RandomNearby(loc), map, ThingPlaceMode.Near);
                    Pawn pawn = thing as Pawn;
                    if (pawn != null)
                    {
                        pawn.TakeDamage(new DamageInfo(DamageDefOf.Bomb, 250, 0, -1, parent));
                    }
                }
            }

            Find.LetterStack.ReceiveLetter("TraderShipsCrashName".Translate(), "TraderShipsCrash".Translate(tradeShip.name, tradeShip.def.label), LetterDefOf.NeutralEvent, new LookTargets(loc, map), tradeShip.Faction);
        }

        private static readonly Texture2D SendAwayTexture = ContentFinder<Texture2D>.Get("TraderShips/SendAway", true);
        private static readonly Texture2D TradeCommandTex = ContentFinder<Texture2D>.Get("UI/Commands/FulfillTradeRequest", true);
    }
}
