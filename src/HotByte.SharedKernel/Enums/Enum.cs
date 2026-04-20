using System.Text.Json.Serialization;

namespace HotByte.SharedKernel.Enums
{
    public class Enum
    {
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public enum UserRole { Customer, RestaurantOwner, Admin }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public enum UserStatus { Active, Inactive }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public enum OrderStatus { Placed, Confirmed, Preparing, OutForDelivery, Delivered, Cancelled }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public enum PaymentMethod { CreditCard, DebitCard, UPI, CashOnDelivery, NetBanking }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public enum PaymentStatus { Pending, Completed, Failed, Refunded }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public enum MealTime { Breakfast, Lunch, Dinner, AllDay }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public enum MenuCategoryType { Appetizer, MainCourse, Dessert, Burger, Pizza, Italian, Arabian, Beverage, Other }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public enum DietaryType { Veg, NonVeg }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public enum TasteInfo { Sweet, SpicyLight, SpicyMedium, SpicyFull, Sour, Savory }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public enum RestaurantStatus { Active, Inactive, Suspended }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public enum NotificationType { OrderPlaced, OrderConfirmed, OrderDispatched, OrderDelivered, OrderCancelled, General }
    }
}
