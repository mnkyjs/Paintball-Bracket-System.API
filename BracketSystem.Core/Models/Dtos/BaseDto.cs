namespace BracketSystem.Core.Models.Dtos
{
    public abstract class BaseDto<T>
    {
        public static void CopyProperties(object source, object target)
        {
            var targetProperties = target.GetType().GetProperties();

            foreach (var sourceProp in source.GetType().GetProperties())
            {
                foreach (var targetProp in targetProperties)
                {
                    if (targetProp.Name == sourceProp.Name &&
                    targetProp.PropertyType.IsAssignableFrom(sourceProp.PropertyType))
                    {
                        targetProp.SetValue(target, sourceProp.GetValue(source, new object[] { }), new object[] { });
                        break;
                    }
                }
            }
        }

        /// <summary>
        ///     Updates the entity from Dto to Entity
        /// </summary>
        /// <param name="entity"></param>
        public abstract void UpdateEntity(T entity);
    }
}