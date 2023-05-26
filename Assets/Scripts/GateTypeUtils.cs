namespace CountMasterClone
{
    public static class GateTypeUtils
    {
        public static string ToValueString(this GateType type)
        {
            switch (type)
            {
                case GateType.Add:
                    return "+";

                case GateType.Multiplication:
                    return "x";

                default:
                    throw new System.ArgumentException("Enum value is undefined!");
            }
        }
    }
}