namespace Discounts.Application.Common.Exceptions
{
    public class ValidationsException : Exception
    {
        public IReadOnlyCollection<string> Errors { get; }

        public ValidationsException(string message)
            : base("One or more validation failures occurred.")
        {
            Errors = new List<string> { message }.AsReadOnly();
        }

        public ValidationsException(IEnumerable<string> errors)
            : base("One or more validation failures occurred.")
        {
            Errors = errors.ToList().AsReadOnly();
        }
    }

}
