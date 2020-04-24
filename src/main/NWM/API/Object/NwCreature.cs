using System;
using System.Collections.Generic;
using NWM.API.Constants;
using NWMX.API;
using NWN;

namespace NWM.API
{
  public class NwCreature : NwGameObject
  {
    internal NwCreature(uint objectId) : base(objectId) {}

    private const int MaxClasses = 3;

    /// <summary>
    /// Creates a creature at the specified location.
    /// </summary>
    /// <param name="template">The creature resref template from the toolset palette</param>
    /// <param name="location">The location where this creature will spawn</param>
    /// <param name="useAppearAnim">If true, plays EffectAppear when created.</param>
    /// <param name="newTag">The new tag to assign this creature. Leave uninitialized/as null to use the template's tag.</param>
    /// <returns></returns>
    public static NwCreature Create(string template, Location location, bool useAppearAnim = false, string newTag = "")
    {
      return CreateInternal<NwCreature>(ObjectType.Item, template, location, useAppearAnim, newTag);
    }

    /// <summary>
    /// Gets a value indicating whether this creature is currently possessed by a DM avatar.
    /// </summary>
    public bool IsDMPossessed
    {
      get => NWScript.GetIsDMPossessed(this).ToBool();
    }

    /// <summary>
    /// Gets the possessor of this creature. This can be the master of a familiar, or the DM for a DM controlled creature.
    /// </summary>
    public NwCreature Master
    {
      get => NWScript.GetMaster(this).ToNwObject<NwCreature>();
    }

    /// <summary>
    /// Gets or sets the total experience points for this creature, taking/granting levels based on progression.
    /// </summary>
    public int Xp
    {
      get => NWScript.GetXP(this);
      set => NWScript.SetXP(this, value < 0 ? 0 : value);
    }

    public int Level
    {
      get => NWScript.GetHitDice(this);
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
    ///  Determine the number of levels this creature holds in the specified <see cref="ClassType"/>.
    /// </summary>
    public int GetLevelByClass(ClassType classType)
    {
      return NWScript.GetLevelByClass((int) classType, this);
    }

    public IReadOnlyList<ClassType> Classes
    {
      get
      {
        List<ClassType> classes = new List<ClassType>(MaxClasses);
        for (int i = 0; i < MaxClasses; i++)
        {
          ClassType classType = (ClassType) NWScript.GetClassByPosition(i + 1);
          if (classType == ClassType.Invalid)
          {
            break;
          }

          classes.Add(classType);
        }

        return classes.AsReadOnly();
      }
    }

    public int GetSkillRank(Skill skill, bool ranksOnly = false)
    {
      return NWScript.GetSkillRank((int) skill, this, ranksOnly.ToInt());
    }

    /// <summary>
    /// Returns true if this creature knows the specified <see cref="Feat"/>, and can use it.<br/>
    /// Use <see cref="NwCreatureExtensions.KnowsFeat"/> to simply check if a creature knows <see cref="Feat"/>, but may or may not have uses remaining.
    /// </summary>
    public bool HasFeatPrepared(Feat feat)
    {
      return NWScript.GetHasFeat((int) feat, this).ToBool();
    }

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
    public void ActionRandomWalk()
    {
      ExecuteOnSelf(NWScript.ActionRandomWalk);
    }

    /// <summary>
    /// Causes the calling creature to start attacking the target using whichever weapon is current equipped.
    /// </summary>
    /// <param name="target">The target object to attack.</param>
    /// <param name="passive">If TRUE, the attacker will not move to attack the target. If we have a melee weapon equipped, we will just stand still.</param>
    public void ActionAttackTarget(NwGameObject target, bool passive = false)
    {
      ExecuteOnSelf(() => NWScript.ActionAttack(target, passive.ToInt()));
    }

    /// <summary>
    /// Commands this creature to walk/run to the specified destination. If the location is invalid or a path cannot be found to it, the command does nothing.
    /// </summary>
    /// <param name="destination">The location to move towards.</param>
    /// <param name="run">If this is TRUE, the creature will run rather than walk</param>
    public void ActionMoveToLocation(Location destination, bool run = false)
    {
      ExecuteOnSelf(() => NWScript.ActionMoveToLocation(destination, run.ToInt()));
    }

    /// <summary>
    /// Commands this creature to move to a certain distance from the target object.
    /// If there is no path to the object, this command will do nothing.
    /// </summary>
    /// <param name="target">The object we wish the creature to move to</param>
    /// <param name="run">If this is TRUE, the action subject will run rather than walk</param>
    /// <param name="range">This is the desired distance between the creature and the target object</param>
    public void ActionMoveToObject(NwObject target, bool run = false, float range = 1.0f)
    {
      ExecuteOnSelf(() => NWScript.ActionMoveToObject(target, run.ToInt(), range));
    }

    /// <summary>
    /// Commands this creature to move to a certain distance away from fleeFrom
    /// </summary>
    /// <param name="fleeFrom">The target object we wish the creature to move away from. If fleeFrom is not in the same area as the creature, nothing will happen.</param>
    /// <param name="run">If this is TRUE, the creature will run rather than walk</param>
    /// <param name="range">This is the distance we wish the creature to put between themselves and target</param>
    public void ActionMoveAwayFromObject(NwObject fleeFrom, bool run, float range = 40.0f)
    {
      ExecuteOnSelf(() => NWScript.ActionMoveAwayFromObject(fleeFrom, run.ToInt(), range));
    }

    /// <summary>
    /// Causes the creature to move away or flee from location.
    /// </summary>
    public void ActionMoveAwayFromLocation(Location location, bool run, float range = 40.0f)
    {
      ExecuteOnSelf(() => NWScript.ActionMoveAwayFromLocation(location, run.ToInt(), range));
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
    /// Adds the specified item to the creature's inventory.
    /// </summary>
    /// <param name="item">The item to add.</param>
    public void GiveItem(NwItem item)
    {
      ExecuteOnSelf(() => NWScript.ActionGiveItem(item, this));
    }

    /// <summary>
    /// Get the item possessed by this creature with the tag itemTag
    /// </summary>
    public NwItem FindItemWithTag(string itemTag)
    {
      return NWScript.GetItemPossessedBy(this, itemTag).ToNwObject<NwItem>();
    }

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
    public void ActionEquipItem(NwItem item, InventorySlot slot)
    {
      ExecuteOnSelf(() => NWScript.ActionEquipItem(item, (int) slot));
    }

    /// <summary>
    /// Commands this creature to unequip the specified item from whatever slot it is currently in.
    /// </summary>
    public void ActionUnequipItem(NwItem item)
    {
      ExecuteOnSelf(() => NWScript.ActionUnequipItem(item));
    }

    /// <summary>
    /// Commands this creature to walk over, and pick up the specified item on the ground.
    /// </summary>
    /// <param name="item">The item to pick up.</param>
    public void ActionPickUpItem(NwItem item)
    {
      ExecuteOnSelf(() => NWScript.ActionPickUpItem(item));
    }

    /// <summary>
    /// Commands this creature to begin placing down an item at its feet.
    /// </summary>
    /// <param name="item">The item to drop.</param>
    public void ActionPutDownItem(NwItem item)
    {
      ExecuteOnSelf(() => NWScript.ActionPutDownItem(item));
    }

    /// <summary>
    /// Commands the creature to sit in the specified placeable.
    /// </summary>
    /// <param name="sitPlaceable">The placeable to sit in. Must be marked useable, empty, and support sitting (e.g. chairs)</param>
    public void ActionSit(NwPlaceable sitPlaceable)
    {
      ExecuteOnSelf(() => NWScript.ActionSit(sitPlaceable));
    }
  }
}