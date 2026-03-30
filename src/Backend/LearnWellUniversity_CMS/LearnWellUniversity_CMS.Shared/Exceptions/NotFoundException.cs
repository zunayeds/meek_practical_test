namespace LearnWellUniversity_CMS.Shared.Exceptions;

public class NotFoundException(string message, Exception? originalException = null) : Exception(message, originalException);
