using System;
using NWN.Core.NWNX;
using NWNX.API.Constants;

namespace NWN.API
{
  public abstract class LocalVariable
  {
    public string Name { get; protected set; }

    public NwObject Object { get; protected set; }

    internal static LocalVariable Create(NwObject nwObject, Core.NWNX.LocalVariable variable)
    {
      LocalVarType varType = (LocalVarType)variable.type;
      return varType switch
      {
        LocalVarType.Int => LocalVariable<int>.Create(nwObject, variable.key),
        LocalVarType.Float => LocalVariable<float>.Create(nwObject, variable.key),
        LocalVarType.String => LocalVariable<string>.Create(nwObject, variable.key),
        LocalVarType.Object => LocalVariable<NwObject>.Create(nwObject, variable.key),
        LocalVarType.Location => LocalVariable<Location>.Create(nwObject, variable.key),
        _ => throw new ArgumentOutOfRangeException(),
      };
    }

    public abstract void Delete();
  }

  /// <summary>
  /// Represents a local variable stored on an Object.
  /// </summary>
  public class LocalVariable<T> : LocalVariable, IEquatable<LocalVariable<T>>
  {
    private ILocalVariableConverter<T> converter;

    private LocalVariable() {}

    internal static LocalVariable<T> Create(NwObject instance, string name)
    {
      LocalVariable<T> variable = new LocalVariable<T>();
      variable.Name = name;
      variable.Object = instance;
      variable.converter = VariableConverterService.GetLocalConverter<T>();

      return variable;
    }

    /// <summary>
    /// Gets or sets the current value of this variable. Returns the default value of T if unassigned (null or 0).
    /// </summary>
    public T Value
    {
      get => converter.GetLocal(Object, Name);
      set => converter.SetLocal(Object, Name, value);
    }

    /// <summary>
    /// Gets a value indicating whether this variable has no value.
    /// </summary>
    public bool HasNothing => !HasValue;

    /// <summary>
    /// Gets a value indicating whether this variable has a value.
    /// </summary>
    public bool HasValue
    {
      get
      {
        int localCount = ObjectPlugin.GetLocalVariableCount(Object);
        for (int i = 0; i < localCount; i++)
        {
          Core.NWNX.LocalVariable variable = ObjectPlugin.GetLocalVariable(Object, i);
          if (variable.key == Name)
          {
            return true;
          }
        }

        return false;
      }
    }

    /// <summary>
    /// Implicit conversion of the value of this variable.
    /// </summary>
    public static implicit operator T(LocalVariable<T> value)
    {
      return value.Value;
    }

    /// <summary>
    /// Deletes the value of this variable.
    /// </summary>
    public override void Delete() => converter.ClearLocal(Object, Name);

    public bool Equals(LocalVariable<T> other)
    {
      if (ReferenceEquals(null, other))
      {
        return false;
      }

      if (ReferenceEquals(this, other))
      {
        return true;
      }

      return Equals(Value, other.Value);
    }

    public override bool Equals(object obj)
    {
      if (ReferenceEquals(null, obj))
      {
        return false;
      }

      if (ReferenceEquals(this, obj))
      {
        return true;
      }

      if (obj.GetType() != this.GetType())
      {
        return false;
      }

      return Equals((LocalVariable<T>) obj);
    }

    public override int GetHashCode()
    {
      return Value != null ? Value.GetHashCode() : 0;
    }

    public static bool operator ==(LocalVariable<T> left, LocalVariable<T> right)
    {
      return Equals(left, right);
    }

    public static bool operator !=(LocalVariable<T> left, LocalVariable<T> right)
    {
      return !Equals(left, right);
    }
  }
}
