using Microsoft.AspNetCore.Components;
using System;

namespace source.Components.Elements
{
    public partial class Modal
    {
        [Parameter] public RenderFragment? ChildContent { get; set; }
        [Parameter] public EventCallback OnClose { get; set; }

        private bool IsVisible { get; set; } = false;

        public void Show() {
            IsVisible = true;
            Console.WriteLine("Modal.Show() called. IsVisible is now true."); 
            StateHasChanged(); 
        }

        public void Close() {
            IsVisible = false;
            Console.WriteLine("Modal.Close() called. IsVisible is now false.");
            StateHasChanged();

            if (OnClose.HasDelegate) {
                OnClose.InvokeAsync();
            }
        }
    }
}