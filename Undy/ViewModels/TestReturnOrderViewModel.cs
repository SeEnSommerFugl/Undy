using Microsoft.Data.SqlClient;
using System;
using Undy.ViewModels.Helpers;
using System.Windows.Input;
using Undy.Data.Repository;
using Undy.Models;

namespace Undy.ViewModels
{
    public class TestReturnOrderViewModel : BaseViewModel
    {
        private IBaseRepository<ReturnOrder, Guid> _tempReturnRepo;

        private string? _orderNumber;
        public string? OrderNumber
        {
            get => _orderNumber;
            set
            {
                if (SetProperty(ref _orderNumber, value));
            }
        }

        private string _email;
        public string Email
        {
            get => _email;
            set
            {
                if (SetProperty(ref _email, value));


            }
        }
        private string? _reason;
        public string? Reason
        {
            get => _reason;
            set 
            {
                if (SetProperty(ref _reason, value)) ;
            }
        }

        private string? _statusMessage;
        public string? StatusMessage
        {
            get => _statusMessage;
            set => SetProperty(ref _statusMessage, value);
        }

        public ICommand ConfirmCommand {  get; }

        public TestReturnOrderViewModel(IBaseRepository<ReturnOrder, Guid> tempReturnRepo)
        {
            _tempReturnRepo = tempReturnRepo;
            
            ConfirmCommand = new RelayCommand(ConfirmAction, CanConfirm);
        }

        private bool CanConfirm(object? parameter)
        {
            return !string.IsNullOrWhiteSpace(OrderNumber) && 
                   !string.IsNullOrWhiteSpace(Email) && 
                   !string.IsNullOrWhiteSpace(Reason);
        }

        private async void ConfirmAction(object? parameter)
        {

            if (!Guid.TryParse(OrderNumber, out Guid salesOrderId))
            {
                StatusMessage = "Ordrenummer er ikke gyldigt. Tjek at du har indtastet korrekt format.";
                return;
            }

            // Logik til håndtering af bekræftelsen af ​​returordren
            var newReturnOrder = new ReturnOrder
            {
                ReturnOrderID = Guid.NewGuid(),
                ReturnOrderDate = DateOnly.FromDateTime(DateTime.Now),
                SalesOrderID = Guid.Parse(OrderNumber) 
            };
            //_tempReturnRepo.Add(newReturnOrder);
            // Yderligere logik, såsom at underrette brugeren, kan tilføjes her.

            await _tempReturnRepo.AddAsync(newReturnOrder);

            StatusMessage = "Returneringen er nu registreret.";

            OrderNumber = string.Empty;
            Email = string.Empty;
            Reason = string.Empty;
        }
    }
}

