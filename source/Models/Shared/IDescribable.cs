namespace source.Models.Shared
{
    public interface IDescribable
    {
        /// <summary>
        /// Returns a dictionary containing the description of the object.
        /// Its keys are the names of the properties, and its values are the descriptions.
        /// </summary>
        Dictionary<String, String> GetDescription();
    }
}
