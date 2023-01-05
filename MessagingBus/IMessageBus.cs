using Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessagingBus
{
    public interface IMessageBus
    {
        Task PublishMessage(IntegrationBaseMessage message, string topicName, string correlationId = null);
    }
}
