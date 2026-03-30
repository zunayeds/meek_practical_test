namespace LearnWellUniversity_CMS.Shared.Exceptions;

public class AlreadyExistException(string message, Exception? originalException = null) : Exception(message, originalException);
