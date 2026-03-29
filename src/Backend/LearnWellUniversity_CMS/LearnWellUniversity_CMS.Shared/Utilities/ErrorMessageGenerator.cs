namespace LearnWellUniversity_CMS.Shared.Utilities;

public static class ErrorMessageGenerator
{
    public static string NotFoundErrorMessage<T>() where T : class
    {
        return $"Cannot find {typeof(T).Name.ToLower()} with provided Id";
    }

    public static string AlreadyExistErrorMessage<T>(string propertyName) where T : class
    {
        return $"Another {typeof(T).Name.ToLower()} already exists with the same {propertyName}";
    }
}
