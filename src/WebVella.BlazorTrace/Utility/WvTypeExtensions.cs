public static class TfTypeExtensions
{
	public static bool InheritsClass(this Type type, Type baseType)
	{
		if (type is null)
			throw new ArgumentException("The provided type is null.", nameof(type));
		if (baseType.IsInterface)
			throw new ArgumentException("The provided type must not be an interface.", nameof(baseType));
		Type currentType = type;
		while (currentType != null)
		{
			if (currentType == baseType)
				return true;

			if(currentType.BaseType is null)
				return false;

			currentType = currentType.BaseType;
		}
		return false;
	}
	

}
