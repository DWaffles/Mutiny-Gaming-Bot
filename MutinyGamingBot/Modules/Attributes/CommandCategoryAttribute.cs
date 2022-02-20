using System;

namespace MutinyBot.Modules
{
    /// <summary>
    /// Defines the category that this command or class should be in when listing the help command.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = false)]
    public class CommandCategoryAttribute : Attribute
    {
        /// <summary>
        /// Gets the category for this command or class
        /// </summary>
        public string CommandCategory { get; }

        /// <summary>
        /// Gives this command or class a category when listing the help command.
        /// </summary>
        /// <param name="category"></param>
        public CommandCategoryAttribute(string category)
        {
            this.CommandCategory = category;
        }
    }
}
