﻿using System;
using CostEffectiveCode.Domain.Cqrs.Commands;
using CostEffectiveCode.Processes.EventArgs;
using CostEffectiveCode.Processes.State;
using JetBrains.Annotations;

namespace CostEffectiveCode.Processes.PubSubProcProtocol
{
    /// <summary>
    /// Here is where <b>PubSubProc [PSP] protocol</b> appears.
    /// It's a way of communication between subsystems using Pub/sub interfaces of CEC framework in terms of Processes of CEC framework.
    ///
    /// This interface represents client-side of PSP protocol.
    /// </summary>
    /// <typeparam name="TProcessOptions"></typeparam>
    /// <typeparam name="TProcessState"></typeparam>
    /// <typeparam name="TProcessException"></typeparam>
    /// <typeparam name="TProcessResult"></typeparam>
    [PublicAPI]
    public interface IProcessesProtocolClient<TProcessOptions, TProcessState, TProcessException, TProcessResult>
        where TProcessState : ProcessState
        where TProcessException : ProcessExceptionBase, new()
    {
        void StartProcess(StartProcessEventArgs<TProcessOptions> message);

        void SubscribeOnStateChanged(ICommand<ProcessStateChangedEventArgs<TProcessState>> handler);

        void SubscribeOnFailure(ICommand<ProcessFailedEventArgs<TProcessException>> handler);

        void SubscribeOnFinish(ICommand<ProcessFinishedEventArgs<TProcessResult>> handler);
    }

    [PublicAPI]
    public static class ProcessesProtocolClientExtensions
    {
        public static Guid StartProcess<TProcessOptions, TProcessState, TProcessException, TProcessResult>(
            this IProcessesProtocolClient<TProcessOptions, TProcessState, TProcessException, TProcessResult> client,
            TProcessOptions options)
            where TProcessState : ProcessState
            where TProcessException : ProcessExceptionBase, new()
        {
            var message = new StartProcessEventArgs<TProcessOptions>(options);
            client.StartProcess(message);

            return message.ProcessGuid;
        }

        public static void SubscribeOnStateChanged<TProcessOptions, TProcessState, TProcessException, TProcessResult>(
            this IProcessesProtocolClient<TProcessOptions, TProcessState, TProcessException, TProcessResult> client,
            Action<ProcessStateChangedEventArgs<TProcessState>> handler)
            where TProcessState : ProcessState
            where TProcessException : ProcessExceptionBase, new()
        {
            client.SubscribeOnStateChanged(new ActionCommand<ProcessStateChangedEventArgs<TProcessState>>(handler));
        }

        public static void SubscribeOnFailure<TProcessOptions, TProcessState, TProcessException, TProcessResult>(
            this IProcessesProtocolClient<TProcessOptions, TProcessState, TProcessException, TProcessResult> client,
            Action<ProcessFailedEventArgs<TProcessException>> handler)
            where TProcessState : ProcessState
            where TProcessException : ProcessExceptionBase, new()
        {
            client.SubscribeOnFailure(new ActionCommand<ProcessFailedEventArgs<TProcessException>>(handler));
        }

        public static void SubscribeOnFinish<TProcessOptions, TProcessState, TProcessException, TProcessResult>(
            this IProcessesProtocolClient<TProcessOptions, TProcessState, TProcessException, TProcessResult> client,
            Action<ProcessFinishedEventArgs<TProcessResult>> handler)
            where TProcessState : ProcessState
            where TProcessException : ProcessExceptionBase, new()
        {
            client.SubscribeOnFinish(new ActionCommand<ProcessFinishedEventArgs<TProcessResult>>(handler));
        }
    }
}