using Ookii.CommandLine;

namespace HeadlessRDP
{
    class SwitchParameters
    {
        [CommandLineArgument(IsRequired = false)]
        public string Server { get; set; }
        [CommandLineArgument(IsRequired = false)]
        public string UserName { get; set; }
        [CommandLineArgument(IsRequired = false)]
        public string Domain { get; set; }
        [CommandLineArgument(IsRequired = false)]
        public string Password { get; set; }
    }
}
