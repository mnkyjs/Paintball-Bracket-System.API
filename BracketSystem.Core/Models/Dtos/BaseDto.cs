namespace BracketSystem.Core.Models.Dtos
{
    public abstract class BaseDto
    {
        /// <summary>
        /// Convenience function for copying all matching (by property name) properties from the given source object to the given target object.
        /// </summary>
        /// <param name="source">Source object from which to copy properties from</param>
        /// <param name="target">Target object to which to copy properties to</param>
        ///
        public static void CopyProperties(object source, object target)
        {
            var targetProperties = target.GetType().GetProperties();

            foreach (var sourceProp in source.GetType().GetProperties())
            foreach (var targetProp in targetProperties)
                if (targetProp.Name == sourceProp.Name &&
                    targetProp.PropertyType.IsAssignableFrom(sourceProp.PropertyType))
                {
                    targetProp.SetValue(target, sourceProp.GetValue(source, new object[] { }), new object[] { });
                    break;
                }
        }
    }
}