using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace heat_production_optimization.Models
{
    [PrimaryKey("MessageType")]
    public class UiMessagesDataModel
    {
        [Key]
        public MessageType MessageType { get; set; }
        public string Message { get; set; } = string.Empty;

        public UiMessagesDataModel(MessageType messageType, string message) 
        {
            MessageType = messageType;
            Message = message;
        }
    }

    public enum MessageType
    {
        DataUploadError,
        DataUploadPath,
        OptimizerError
    }
}
