using GameOfLife.ViewModel;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GameOfLife.Views
{
    public class GameOfLifeBase:ComponentBase
    {
        [Inject] public GameOfLifeViewModel ViewModel { get; set; }

        private void NotifyChanged(object vm, EventArgs e) => InvokeAsync(StateHasChanged);
        protected override async void OnInitialized()
        {
            ViewModel.PropertyChanged += NotifyChanged;
            await InvokeAsync(StateHasChanged);
        }
    }
}
