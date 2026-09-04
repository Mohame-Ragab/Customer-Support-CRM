namespace CustomerSupportCRM.Domain.Enums;

public enum EmailDeliveryStatus
{
    Pending = 0,
    Sent = 1,
    Failed = 2,
    Received = 3, // inbound
}
