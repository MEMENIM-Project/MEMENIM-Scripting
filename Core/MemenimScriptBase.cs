﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reflection;

namespace Memenim.Scripting.Core
{
    [RequiredMemenimVersion("0.15.10", null)]
    public abstract class MemenimScriptBase
    {
        public ReadOnlyDictionary<string, MemenimScriptCommand> Commands { get; }

        protected MemenimScriptBase()
        {
            Commands = GetRegisteredCommands();
        }

        private ReadOnlyDictionary<string, MemenimScriptCommand> GetRegisteredCommands()
        {
            var commands = new Dictionary<string, MemenimScriptCommand>();

            foreach (var methodInfo in GetType().GetMethods())
            {
                if (!Attribute.IsDefined(methodInfo, typeof(MemenimScriptCommandAttribute)))
                    continue;

                var commandAttribute = (MemenimScriptCommandAttribute)methodInfo
                    .GetCustomAttribute(typeof(MemenimScriptCommandAttribute));

                var commandName = commandAttribute?.Name;
                var commandDescription = commandAttribute?.Description;

                if (string.IsNullOrWhiteSpace(commandName))
                    commandName = methodInfo.Name;
                if (string.IsNullOrWhiteSpace(commandDescription))
                    commandDescription = string.Empty;

                commands.Add(commandName, new MemenimScriptCommand(
                    methodInfo, commandName, commandDescription));
            }

            return new ReadOnlyDictionary<string, MemenimScriptCommand>(
                commands);
        }

        public bool MemenimVersionIsSatisfied(MemenimVersion version)
        {
            var type = GetType();

            if (!Attribute.IsDefined(type, typeof(RequiredMemenimVersionAttribute)))
                type = typeof(MemenimScriptBase);

            var requiredVersionAttribute = (RequiredMemenimVersionAttribute)type
                .GetCustomAttribute(typeof(RequiredMemenimVersionAttribute));

            if (requiredVersionAttribute == null)
                return true;

            return requiredVersionAttribute.Range
                .IsSatisfied(version);
        }
    }
}