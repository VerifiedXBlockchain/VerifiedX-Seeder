using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace RBXOSeed.Utility
{
    public class ArgumentParser
    {
        public static async Task ProcessArgs(string[] args)
        {
            // Check if there are arguments
            if (args.Length > 0)
            {
                // Loop through each argument
                foreach (string arg in args)
                {
                    // Check argument type and process accordingly
                    if (arg.StartsWith("--"))
                    {
                        // Handle named arguments (prefixed with --)
                        string[] parts = arg.Split('=');
                        string key = parts[0].Substring(2); // Remove --
                        string? value = parts.Length > 1 ? parts[1] : null;
                        AssignArgument(key, value);
                    }
                    else if(arg.StartsWith("-"))
                    {
                        // Handle shorthand arguments (single-character flags)
                        // Your logic for single-character flags here
                        string flag = arg.Substring(1); // Remove the -
                        AssignArgument("0", flag);
                    }
                    else
                    {
                        //not a valid argument
                    }
                }
            }
        }

        private static void AssignArgument(string key, string? value)
        {
            if (key == "0")
            {
                if(value != null)
                {
                    var val = value.ToLower();
                    switch(val)
                    {
                        case "seednodes" :
                            Globals.CommandArguments.SeedNodes = true;
                            break;
                        case "useselfssl":
                            Globals.CommandArguments.UseSelfSignedCertificate = true;
                            break;
                        default:
                            break;
                    }
                }
            }
            else
            {

            }
        }
    }
}
