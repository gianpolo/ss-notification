public record BasketItemAddedIntegrationEvent(int GuideId, string GuideName, string ItemId, string ItemName) : IntegrationEvent;
