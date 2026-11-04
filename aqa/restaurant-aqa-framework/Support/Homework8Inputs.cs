namespace Restaurant_Aqa_Framework.Support;

public static class Homework8Inputs
{
    public static InputField Field(string name) => name switch
    {
        "first name" => InputField.FirstName,
        "last name" => InputField.LastName,
        "email" => InputField.EmailAddress,
        "password" => InputField.Password,
        "password confirmation" => InputField.PasswordConfirmation,
        _ => throw new ArgumentOutOfRangeException(nameof(name), name, "Unknown homework field."),
    };

    public static string AtLength(string field, int length) => field switch
    {
        "password" when length >= 4 => "Aa1!" + new string('a', length - 4),
        "first name" or "last name" => new string('a', length),
        // Local part <= 64; each domain label <= 63. Only total length varies.
        "email" when length is 254 or 255 =>
            new string('a', 64) + "@" + new string('b', 62) + "." + new string('c', 62)
            + "." + new string('d', length - 195) + ".com",
        _ => throw new ArgumentOutOfRangeException(nameof(length)),
    };
}
