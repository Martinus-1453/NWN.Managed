using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NWN.API.Constants;
using NWN.Core;
using NWNX.API;
using NWNX.API.Constants;
using Action = NWN.API.Constants.Action;

namespace NWN.API
{
  [NativeObjectInfo(ObjectType.Creature, InternalObjectType.Creature)]
  public class NwCreature : NwGameObject
  {
    internal NwCreature(uint objectId) : base(objectId) {}

    /// <summary>
    /// Gets if this creature is a dead NPC, dead PC, or dying PC.
    /// </summary>
    public bool IsDead => NWScript.GetIsDead(this).ToBool();

    /// <summary>
    /// Gets if this creature is in combat.
    /// </summary>
    public bool IsInCombat => NWScript.GetIsInCombat(this).ToBool();

    /// <summary>
    /// Gets the racial type of this creature.
    /// </summary>
    public RacialType RacialType => (RacialType) NWScript.GetRacialType(this);

    /// <summary>
    /// Gets the gender of this creature.
    /// </summary>
    public Gender Gender => (Gender) NWScript.GetGender(this);

    /// <summary>
    /// Gets whether this creature is currently resting.
    /// </summary>
    public bool IsResting => NWScript.GetIsResting(this).ToBool();

    /// <summary>
    /// Gets whether this creature is a playable racial type.
    /// </summary>
    public bool IsPlayableRace => NWScript.GetIsPlayableRacialType(this).ToBool();

    /// <summary>
    /// Gets the total weight of this creature, in pounds.
    /// </summary>
    public decimal TotalWeight => NWScript.GetWeight(this) * 0.1m;

    /// <summary>
    /// Gets this creature's armour class.
    /// </summary>
    public int AC => NWScript.GetAC(this);

    /// <summary>
    /// Gets this creature's age, in years.
    /// </summary>
    public int Age => NWScript.GetAge(this);

    /// <summary>
    /// Gets the size of this creature.
    /// </summary>
    public CreatureSize Size => (CreatureSize) NWScript.GetCreatureSize(this);

    /// <summary>
    /// Gets or sets the name of this creature's deity.
    /// </summary>
    public string Deity
    {
      get => NWScript.GetDeity(this);
      set => NWScript.SetDeity(this, value);
    }

    /// <summary>
    /// Gets or sets the name of this creature's sub-race.
    /// </summary>
    public string SubRace
    {
      get => NWScript.GetSubRace(this);
      set => NWScript.SetSubRace(this, value);
    }

    /// <summary>
    /// Gets or sets this creature's currently set Phenotype (body type).
    /// </summary>
    public Phenotype Phenotype
    {
      get => (Phenotype) NWScript.GetPhenoType(this);
      set => NWScript.SetPhenoType((int) value, this);
    }

    /// <summary>
    /// Gets the calculated challenge rating for this creature.
    /// </summary>
    public float ChallengeRating => NWScript.GetChallengeRating(this);

    /// <summary>
    /// Returns the movement rate of this creature.
    /// </summary>
    public MovementRate MovementRate => (MovementRate) NWScript.GetMovementRate(this);

    /// <summary>
    /// Gets this creature's Law/Chaos Alignment.
    /// </summary>
    public Alignment LawChaosAlignment => (Alignment) NWScript.GetAlignmentLawChaos(this);

    /// <summary>
    /// Gets this creature's Good/Evil Alignment.
    /// </summary>
    public Alignment GoodEvilAlignment => (Alignment) NWScript.GetAlignmentGoodEvil(this);

    /// <summary>
    /// Gets a value indicating whether this creature is currently possessed by a DM avatar.
    /// </summary>
    public bool IsDMPossessed => NWScript.GetIsDMPossessed(this).ToBool();

    /// <summary>
    /// Gets if this creature is a familiar currently possessed by a master.
    /// </summary>
    public bool IsPossessedFamiliar => NWScript.GetIsPossessedFamiliar(this).ToBool();

    /// <summary>
    /// Gets the last command issued to this creature.
    /// </summary>
    public AssociateCommand LastCommandFromMaster => (AssociateCommand) NWScript.GetLastAssociateCommand(this);

    /// <summary>
    /// Gets the possessor of this creature. This can be the master of a familiar, or the DM for a DM controlled creature.
    /// </summary>
    public NwCreature Master => NWScript.GetMaster(this).ToNwObject<NwCreature>();

    /// <summary>
    /// Returns this creature's current attack target.
    /// </summary>
    public NwGameObject AttackTarget => NWScript.GetAttackTarget(this).ToNwObject<NwGameObject>();

    /// <summary>
    /// Gets the current action that this creature is executing.
    /// </summary>
    public Action CurrentAction => (Action) NWScript.GetCurrentAction(this);

    /// <summary>
    /// Gets the caster level of the last spell this creature casted.
    /// </summary>
    public int LastSpellCasterLevel => NWScript.GetCasterLevel(this);

    /// <summary>
    /// Returns the Hit Dice/Level of this creature.
    /// </summary>
    public int Level => NWScript.GetHitDice(this);

    /// <summary>
    /// Gets or sets the total experience points for this creature, taking/granting levels based on progression.
    /// </summary>
    public int Xp
    {
      get => NWScript.GetXP(this);
      set => NWScript.SetXP(this, value < 0 ? 0 : value);
    }

    /// <summary>
    /// Gets or sets whether this creature's action queue can be modified.
    /// </summary>
    public bool Commandable
    {
      get => NWScript.GetCommandable(this).ToBool();
      set => NWScript.SetCommandable(value.ToInt(), this);
    }

    /// <summary>
    /// Gets this creature's classes.
    /// </summary>
    public IReadOnlyList<ClassType> Classes
    {
      get
      {
        const int maxClasses = 3;

        List<ClassType> classes = new List<ClassType>(maxClasses);
        for (int i = 1; i <= maxClasses; i++)
        {
          ClassType classType = (ClassType) NWScript.GetClassByPosition(i, this);
          if (classType == ClassType.Invalid)
          {
            break;
          }

          classes.Add(classType);
        }

        return classes.AsReadOnly();
      }
    }

    /// <summary>
    /// Gets all effects (permanent and temporary) that are active on this creature.
    /// </summary>
    public IEnumerable<Effect> ActiveEffects
    {
      get
      {
        for (Effect effect = NWScript.GetFirstEffect(this); NWScript.GetIsEffectValid(effect) == true.ToInt(); effect = NWScript.GetNextEffect(this))
        {
          yield return effect;
        }
      }
    }

    /// <summary>
    /// Gets or sets this creature's Chaos (0) - Lawful (100) alignment value.
    /// </summary>
    public int LawChaosValue
    {
      get => NWScript.GetLawChaosValue(this);
      set
      {
        int current = LawChaosValue;
        if (value == current)
        {
          return;
        }

        Alignment alignment = value < current ? Alignment.Chaotic : Alignment.Lawful;
        int shift = Math.Abs(value - current);
        NWScript.AdjustAlignment(this, (int) alignment, shift, false.ToInt());
      }
    }

    /// <summary>
    /// Gets or sets this creature's Evil (0) - Good (100) alignment value.
    /// </summary>
    public int GoodEvilValue
    {
      get => NWScript.GetGoodEvilValue(this);
      set
      {
        int current = GoodEvilValue;
        if (value == current)
        {
          return;
        }

        Alignment alignment = value < current ? Alignment.Evil : Alignment.Good;
        int shift = Math.Abs(value - current);
        NWScript.AdjustAlignment(this, (int) alignment, shift, false.ToInt());
      }
    }

    /// <summary>
    /// Gets or sets the amount of gold carried by this creature.
    /// </summary>
    public int Gold
    {
      get => NWScript.GetGold(this);
      set
      {
        int diff = value - Gold;
        if (diff == 0)
        {
          return;
        }

        if (diff > 0)
        {
          NWScript.GiveGoldToCreature(this, diff);
        }
        else
        {
          NWScript.TakeGoldFromCreature(Math.Abs(diff), this, true.ToInt());
        }
      }
    }

    /// <summary>
    /// Creates a creature at the specified location.
    /// </summary>
    /// <param name="template">The creature resref template from the toolset palette</param>
    /// <param name="location">The location where this creature will spawn</param>
    /// <param name="useAppearAnim">If true, plays EffectAppear when created.</param>
    /// <param name="newTag">The new tag to assign this creature. Leave uninitialized/as null to use the template's tag.</param>
    /// <returns></returns>
    public static NwCreature Create(string template, Location location, bool useAppearAnim = false, string newTag = "") => 
      NwObjectFactory.CreateInternal<NwCreature>(template, location, useAppearAnim, newTag);

    /// <summary>
    /// Gets the item that is equipped in the specified inventory slot.
    /// </summary>
    /// <param name="slot">The inventory slot to check.</param>
    /// <returns>The item in the inventory slot, otherwise null if it is unpopulated.</returns>
    public NwItem GetItemInSlot(InventorySlot slot) => NWScript.GetItemInSlot((int) slot, this).ToNwObject<NwItem>();

    /// <summary>
    /// Attempts to perform a melee touch attack on target. This is not a creature action, and assumes that this creature is already within range of the target.
    /// </summary>
    /// <param name="target">The target of this touch attack.</param>
    public async Task<TouchAttackResult> TouchAttackMelee(NwGameObject target)
    {
      await WaitForObjectContext();
      return (TouchAttackResult) NWScript.TouchAttackMelee(target);
    }

    /// <summary>
    /// Attempts to perform a ranged touch attack on the target. This is not a creature action, and simply does a roll to see if the target was hit.
    /// </summary>
    /// <param name="target">The target of this touch attack.</param>
    /// <param name="displayFeedback">If true, displays combat feedback in the chat window.</param>
    public async Task<TouchAttackResult> TouchAttackRanged(NwGameObject target, bool displayFeedback)
    {
      await WaitForObjectContext();
      return (TouchAttackResult) NWScript.TouchAttackRanged(target, displayFeedback.ToInt());
    }

    /// <summary>
    /// Adjusts the alignment of this creature and associated party members by the specified value.<br/>
    /// Use the <see cref="LawChaosValue"/> and <see cref="GoodEvilValue"/> setters to only affect this creature.
    /// </summary>
    /// <param name="alignment">The alignment to shift towards.</param>
    /// <param name="shift">The amount of alignment shift.</param>
    public void AdjustPartyAlignment(Alignment alignment, int shift) => NWScript.AdjustAlignment(this, (int) alignment, shift);

    /// <summary>
    /// Gets the specified ability score from this creature.
    /// </summary>
    /// <param name="ability">The type of ability.</param>
    /// <param name="baseOnly">If true, will return the creature's base ability score without bonuses or penalties.</param>
    /// <returns></returns>
    public int GetAbilityScore(Ability ability, bool baseOnly = false) => NWScript.GetAbilityScore(this, (int) ability, baseOnly.ToInt());

    /// <summary>
    /// Gets the DC to save against for a spell (10 + spell level + relevant ability bonus).
    /// </summary>
    public async Task<int> GetSpellSaveDC()
    {
      await WaitForObjectContext();
      return NWScript.GetSpellSaveDC();
    }

    /// <summary>
    /// Returns the last target this creature tried to attack. Reset at the end of combat.
    /// </summary>
    public async Task<NwGameObject> GetAttemptedAttackTarget()
    {
      await WaitForObjectContext();
      return NWScript.GetAttemptedAttackTarget().ToNwObject<NwGameObject>();
    }

    /// <summary>
    /// Gets the target object of this creature's last spell.
    /// </summary>
    public async Task<NwGameObject> GetLastSpellTargetObject()
    {
      await WaitForObjectContext();
      return NWScript.GetSpellTargetObject().ToNwObject<NwGameObject>();
    }

    /// <summary>
    /// Gets the target location of this creature's last spell.
    /// </summary>
    public async Task<Location> GetLastSpellTargetLocation()
    {
      await WaitForObjectContext();
      return NWScript.GetSpellTargetLocation();
    }

    /// <summary>
    /// Gets the target this creature attempted to cast a spell at. Reset at the end of combat.
    /// </summary>
    public async Task<NwGameObject> GetAttemptedSpellTarget()
    {
      await WaitForObjectContext();
      return NWScript.GetAttemptedSpellTarget().ToNwObject<NwGameObject>();
    }

    /// <summary>
    ///  Determine the number of levels this creature holds in the specified <see cref="ClassType"/>.
    /// </summary>
    public int GetLevelByClass(ClassType classType) => NWScript.GetLevelByClass((int) classType, this);

    /// <summary>
    /// Gets if this creature has the specified spell available to cast.
    /// </summary>
    /// <param name="spell">The spell to check.</param>
    /// <returns>True if this creature can immediately cast the spell.</returns>
    public bool HasSpellUse(Spell spell) => NWScript.GetHasSpell((int) spell, this) > 0;

    /// <summary>
    /// Gets the number of ranks this creature has in the specified skill.
    /// </summary>
    /// <param name="skill">The skill to check.</param>
    /// <param name="ranksOnly">If true, returns the base amount of skill ranks without any ability modifiers.</param>
    /// <returns>-1 if the creature does not have this skill, 0 if untrained, otherwise the number of skill ranks.</returns>
    public int GetSkillRank(Skill skill, bool ranksOnly = false) => 
      NWScript.GetSkillRank((int) skill, this, ranksOnly.ToInt());

    /// <summary>
    /// Returns true if this creature has the skill specified, and is useable.
    /// </summary>
    /// <param name="skill">The skill to check.</param>
    /// <returns>True if the creature has this skill.</returns>
    public bool HasSkill(Skill skill) => 
      NWScript.GetHasSkill((int) skill, this).ToBool();

    /// <summary>
    /// Returns true if 1d20 + skill rank is greater than, or equal to difficultyClass.
    /// </summary>
    /// <param name="skill">The type of skill check.</param>
    /// <param name="difficultyClass">The DC of this skill check.</param>
    /// <returns></returns>
    public bool DoSkillCheck(Skill skill, int difficultyClass) => 
      NWScript.GetIsSkillSuccessful(this, (int) skill, difficultyClass).ToBool();

    /// <summary>
    /// Returns true if this creature knows the specified <see cref="Feat"/>, and can use it.<br/>
    /// Use <see cref="Creature.KnowsFeat"/> to simply check if a creature knows <see cref="Feat"/>, but may or may not have uses remaining.
    /// </summary>
    public bool HasFeatPrepared(Feat feat) => 
      NWScript.GetHasFeat((int) feat, this).ToBool();

    /// <summary>
    /// Determines whether this creature has the specified talent.
    /// </summary>
    /// <param name="talent">The talent to check.</param>
    /// <returns>True if this creature has talent, otherwise false.</returns>
    public bool HasTalent(Talent talent) => 
      NWScript.GetCreatureHasTalent(talent, this).ToBool();

    /// <summary>
    /// Applies the specified effect to this creature.
    /// </summary>
    /// <param name="durationType"></param>
    /// <param name="effect">The effect to apply.</param>
    /// <param name="duration">If duration type is <see cref="EffectDuration.Temporary"/>, the duration of this effect in seconds.</param>
    public void ApplyEffect(EffectDuration durationType, Effect effect, float duration = 0f)
    {
      NWScript.ApplyEffectToObject((int)durationType, effect, this, duration);
    }

    /// <summary>
    /// Removes the specified effect from this creature.
    /// </summary>
    /// <param name="effect">The existing effect instance.</param>
    public void RemoveEffect(Effect effect)
    {
      NWScript.RemoveEffect(this, effect);
    }

    /// <summary>
    /// The creature will generate a random location near its current location
    /// and pathfind to it. This repeats and never ends, which means it is necessary
    /// to call <see cref="NwObject.ClearActionQueue"/> in order to allow a creature to perform any other action
    /// once BeginRandomWalking has been called.
    /// </summary>
    public async Task ActionRandomWalk()
    {
      await WaitForObjectContext();
      NWScript.ActionRandomWalk();
    }

    /// <summary>
    /// Causes the calling creature to start attacking the target using whichever weapon is current equipped.
    /// </summary>
    /// <param name="target">The target object to attack.</param>
    /// <param name="passive">If TRUE, the attacker will not move to attack the target. If we have a melee weapon equipped, we will just stand still.</param>
    public async Task ActionAttackTarget(NwGameObject target, bool passive = false)
    {
      await WaitForObjectContext();
      NWScript.ActionAttack(target, passive.ToInt());
    }

    /// <summary>
    /// Commands this creature to walk/run to the specified destination. If the location is invalid or a path cannot be found to it, the command does nothing.
    /// </summary>
    /// <param name="destination">The location to move towards.</param>
    /// <param name="run">If this is TRUE, the creature will run rather than walk</param>
    public async Task ActionMoveToLocation(Location destination, bool run = false)
    {
      await WaitForObjectContext();
      NWScript.ActionMoveToLocation(destination, run.ToInt());
    }

    /// <summary>
    /// Commands this creature to move to a certain distance from the target object.
    /// If there is no path to the object, this command will do nothing.
    /// </summary>
    /// <param name="target">The object we wish the creature to move to</param>
    /// <param name="run">If this is TRUE, the action subject will run rather than walk</param>
    /// <param name="range">This is the desired distance between the creature and the target object</param>
    public async Task ActionMoveToObject(NwObject target, bool run = false, float range = 1.0f)
    {
      await WaitForObjectContext();
      NWScript.ActionMoveToObject(target, run.ToInt(), range);
    }

    /// <summary>
    /// Commands this creature to move to a certain distance away from fleeFrom
    /// </summary>
    /// <param name="fleeFrom">The target object we wish the creature to move away from. If fleeFrom is not in the same area as the creature, nothing will happen.</param>
    /// <param name="run">If this is TRUE, the creature will run rather than walk</param>
    /// <param name="range">This is the distance we wish the creature to put between themselves and target</param>
    public async Task ActionMoveAwayFromObject(NwObject fleeFrom, bool run, float range = 40.0f)
    {
      await WaitForObjectContext();
      NWScript.ActionMoveAwayFromObject(fleeFrom, run.ToInt(), range);
    }

    /// <summary>
    /// Causes the creature to move away or flee from location.
    /// </summary>
    public async Task ActionMoveAwayFromLocation(Location location, bool run, float range = 40.0f)
    {
      await WaitForObjectContext();
      NWScript.ActionMoveAwayFromLocation(location, run.ToInt(), range);
    }

    /// <summary>
    /// Forces this creature to follow the specified target until <see cref="NwObject.ClearActionQueue"/> is called.
    /// </summary>
    /// <param name="target">The target to follow.</param>
    /// <param name="distance">The distance to follow the creature at.</param>
    public async Task ActionForceFollowObject(NwGameObject target, float distance)
    {
      await WaitForObjectContext();
      NWScript.ActionForceFollowObject(target, distance);
    }

    /// <summary>
    /// Creates a copy of this creature.
    /// </summary>
    /// <param name="location">The location to place the new creature. Defaults to the current creature's location</param>
    /// <param name="newTag">A new tag to assign to the creature.</param>
    /// <returns></returns>
    public NwCreature Clone(Location location = null, string newTag = null)
    {
      if (location == null)
      {
        location = Location;
      }

      return NWScript.CopyObject(this, location, sNewTag: newTag).ToNwObject<NwCreature>();
    }

    /// <summary>
    /// Moves the specified item to this creature's inventory.
    /// </summary>
    /// <param name="item">The item to add.</param>
    public async Task GiveItem(NwItem item)
    {
      NwObject assignTarget;
      if (item.Possessor != null)
      {
        assignTarget = item.Possessor;
      }
      else
      {
        assignTarget = item.Area;
      }

      if (assignTarget != this)
      {
        await assignTarget.WaitForObjectContext();
        NWScript.ActionGiveItem(item, this);
      }
    }

    /// <summary>
    /// Get the item possessed by this creature with the tag itemTag
    /// </summary>
    public NwItem FindItemWithTag(string itemTag) => NWScript.GetItemPossessedBy(this, itemTag).ToNwObject<NwItem>();

    /// <summary>
    /// Commands the creature to equip the specified item into the given inventory slot.<br/>
    /// Note: If the creature already has an item equipped in the slot specified, it will be unequipped automatically
    /// by the call to EquipItem, and dropped if the creature lacks inventory space.<br/>
    /// In order for EquipItem to succeed the creature must be able to equip the item normally. This means that:<br/>
    /// 1) The item is in the creature's inventory.<br/>
    /// 2) The item must already be identified (if magical).<br/>
    /// 3) The creature has the level required to equip the item (if magical and ILR is on).<br/>
    /// 4) The creature possesses the required feats to equip the item (such as weapon proficiencies).
    /// </summary>
    public async Task ActionEquipItem(NwItem item, InventorySlot slot)
    {
      await WaitForObjectContext();
      NWScript.ActionEquipItem(item, (int) slot);
    }

    /// <summary>
    /// Commands this creature to unequip the specified item from whatever slot it is currently in.
    /// </summary>
    public async Task ActionUnequipItem(NwItem item)
    {
      await WaitForObjectContext();
      NWScript.ActionUnequipItem(item);
    }

    /// <summary>
    /// Commands this creature to walk over, and pick up the specified item on the ground.
    /// </summary>
    /// <param name="item">The item to pick up.</param>
    public async Task ActionPickUpItem(NwItem item)
    {
      await WaitForObjectContext();
      NWScript.ActionPickUpItem(item);
    }

    /// <summary>
    /// Commands this creature to begin placing down an item at its feet.
    /// </summary>
    /// <param name="item">The item to drop.</param>
    public async Task ActionPutDownItem(NwItem item)
    {
      await WaitForObjectContext();
      NWScript.ActionPutDownItem(item);
    }

    /// <summary>
    /// Commands the creature to sit in the specified placeable.
    /// </summary>
    /// <param name="sitPlaceable">The placeable to sit in. Must be marked useable, empty, and support sitting (e.g. chairs)</param>
    public async Task ActionSit(NwPlaceable sitPlaceable)
    {
      await WaitForObjectContext();
      NWScript.ActionSit(sitPlaceable);
    }

    /// <summary>
    /// Returns true if this creature considers the target an enemy.
    /// </summary>
    public bool IsEnemy(NwCreature target) => NWScript.GetIsEnemy(target, this).ToBool();

    /// <summary>
    /// Returns this creature's domains in the specified class. Unless custom content is used, only clerics have domains.
    /// </summary>
    /// <param name="classType">The class with domains.</param>
    /// <returns>An enumeration of this creature's domains.</returns>
    public IEnumerable<Domain> GetClassDomains(ClassType classType = ClassType.Cleric)
    {
      const int error = (int) Domain.Error;
      int classT = (int) classType;

      int i;
      int current;

      for (i = 1, current = NWScript.GetDomain(this, i, classT); current != error; i++, current = NWScript.GetDomain(this, i, classT))
      {
        yield return (Domain) current;
      }
    }

    /// <summary>
    /// Instructs this creature to enable/disable the specified action mode (parry, power attack, expertise, etc)
    /// </summary>
    /// <param name="actionMode">The action mode to toggle.</param>
    /// <param name="status">The new state of the action mode.</param>
    public void SetActionMode(ActionMode actionMode, bool status)
    {
      NWScript.SetActionMode(this, (int) actionMode, status.ToInt());
    }
  }
}