namespace LearnWellUniversity_CMS.Shared.Exceptions;

public class NotFoundException(string message, Exception? originalException) : Exception(message, originalException);
