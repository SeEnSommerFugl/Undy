// Undy/Undy/Models/WholesaleOrderLine.cs
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Undy.Models
{
    public class WholesaleOrderLine : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        private void OnPropertyChanged([CallerMemberName] string? propertyName = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        private bool SetProperty<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, value))
                return false;

            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }

        private Guid _wholesaleOrderID;
        public Guid WholesaleOrderID
        {
            get => _wholesaleOrderID;
            set => SetProperty(ref _wholesaleOrderID, value);
        }

        private Guid _productID;
        public Guid ProductID
        {
            get => _productID;
            set => SetProperty(ref _productID, value);
        }

        private int _quantity;
        public int Quantity
        {
            get => _quantity;
            set
            {
                if (SetProperty(ref _quantity, value))
                    RecalculatePending();
            }
        }

        private decimal _unitPrice;
        public decimal UnitPrice
        {
            get => _unitPrice;
            set => SetProperty(ref _unitPrice, value);
        }

        private int _quantityReceived;
        public int QuantityReceived
        {
            get => _quantityReceived;
            set
            {
                // Keep within [0..Quantity]
                var clamped = Math.Max(0, Math.Min(value, Quantity));

                if (SetProperty(ref _quantityReceived, clamped))
                    RecalculatePending();
            }
        }

        private string _productNumber = "";
        public string ProductNumber
        {
            get => _productNumber;
            set => SetProperty(ref _productNumber, value);
        }

        private string _productName = "";
        public string ProductName
        {
            get => _productName;
            set => SetProperty(ref _productName, value);
        }

        private int _quantityPending;
        public int QuantityPending
        {
            get => _quantityPending;
            set => SetProperty(ref _quantityPending, value);
        }

        private void RecalculatePending()
        {
            // Mirror SQL view: (Quantity - QuantityReceived)
            QuantityPending = Quantity - QuantityReceived;
            OnPropertyChanged(nameof(Key));
        }

        public WholesaleOrderLineKey Key => new(WholesaleOrderID, ProductID);

        public readonly record struct WholesaleOrderLineKey(Guid WholesaleOrderID, Guid ProductID);
    }
}
