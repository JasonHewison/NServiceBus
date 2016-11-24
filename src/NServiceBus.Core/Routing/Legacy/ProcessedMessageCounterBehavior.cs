﻿namespace NServiceBus
{
    using System;
    using System.Threading.Tasks;
    using Pipeline;

    class ProcessedMessageCounterBehavior : IBehavior<IIncomingPhysicalMessageContext, IIncomingPhysicalMessageContext>
    {
        public ProcessedMessageCounterBehavior(ReadyMessageSender readyMessageSender, NotificationSubscriptions subscriptions)
        {
            this.readyMessageSender = readyMessageSender;

            subscriptions.Subscribe<MessageFaulted>(HandleMessageFaulted);
        }

        public async Task Invoke(IIncomingPhysicalMessageContext context, Func<IIncomingPhysicalMessageContext, Task> next)
        {
            await next(context).ConfigureAwait(false);
            await readyMessageSender.MessageProcessed(context.Message.Headers).ConfigureAwait(false);
        }

        Task HandleMessageFaulted(MessageFaulted @event)
        {
            return readyMessageSender.MessageProcessed(@event.Message.Headers);
        }

        ReadyMessageSender readyMessageSender;
    }
}