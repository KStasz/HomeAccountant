﻿using HomeAccountant.Core.Model;
using HomeAccountant.Core.Services;
using Microsoft.AspNetCore.Components;
using Moq;
using Xunit;
using Assert = Xunit.Assert;

namespace HomeAccountant.Core.ViewModels.Tests
{
    public class BillingPeriodViewModelTests
    {
        private Mock<IAlert> _alertServiceMock;
        private Mock<IBillingPeriodService> _billingPeriodServiceMock;
        private Mock<IPubSubService> _pubSubServiceMock;
        private readonly Mock<NavigationManager> _navManagerMock;
        private readonly Mock<IRegisterService> _registerService;
        private BillingPeriodViewModel _viewModel;
        private Mock<IModalDialog<BillingPeriodModel, BillingPeriodModel>> _billingCreateDialogMock;

        public BillingPeriodViewModelTests()
        {
            _billingCreateDialogMock = new Mock<IModalDialog<BillingPeriodModel, BillingPeriodModel>>();
            _alertServiceMock = new Mock<IAlert>();
            _billingPeriodServiceMock = new Mock<IBillingPeriodService>();
            _pubSubServiceMock = new Mock<IPubSubService>();
            _navManagerMock = new Mock<NavigationManager>();
            _registerService = new Mock<IRegisterService>();

            _viewModel = new BillingPeriodViewModel(
                _billingPeriodServiceMock.Object,
                _pubSubServiceMock.Object,
                _registerService.Object,
                _navManagerMock.Object);
            _viewModel.Alert = _alertServiceMock.Object;
            _viewModel.BillingCreateDialog = _billingCreateDialogMock.Object;
        }

        [Fact()]
        public async Task ToggleBillingPeriodAsync_ShouldSkipMethodIfSelectedBillingPeriodIsNull()
        {
            await _viewModel.ToggleBillingPeriodAsync();

            _billingPeriodServiceMock.Verify(
                x => x.OpenBillingPeriodAsync(
                    It.IsAny<int>(),
                    It.IsAny<int>(),
                    It.IsAny<CancellationToken>()),
                Times.Never);

            _billingPeriodServiceMock.Verify(
                x => x.CloseBillingPeriodAsync(
                    It.IsAny<int>(),
                    It.IsAny<int>(),
                    It.IsAny<CancellationToken>()),
                Times.Never);

            _billingPeriodServiceMock.Verify(
                x => x.GetBiilingPeriodsAsync(
                    It.IsAny<int>(),
                    It.IsAny<CancellationToken>()),
                Times.Never);
        }

        [Fact()]
        public async Task ToggleBillingPeriodAsync_ShouldSetSelectedPeriodStatusAsClosed()
        {
            var availableBillingPeriods = new List<BillingPeriodModel>()
            {
                new BillingPeriodModel()
                {
                    Id = 1,
                    Name = "Test 1",
                    Register = null,
                    IsOpen = true,
                    Entries = null,
                    CreationDate = DateTime.UtcNow,
                },
                new BillingPeriodModel()
                {
                    Id = 2,
                    Name = "Test 2",
                    Register = null,
                    IsOpen = true,
                    Entries = null,
                    CreationDate = DateTime.UtcNow.AddDays(-1),
                }
            };

            var refreshedBillingPeriods = new List<BillingPeriodModel>()
            {
                new BillingPeriodModel()
                {
                    Id = 1,
                    Name = "Test 1",
                    Register = null,
                    IsOpen = true,
                    Entries = null,
                    CreationDate = DateTime.UtcNow,
                },
                new BillingPeriodModel()
                {
                    Id = 2,
                    Name = "Test 2",
                    Register = null,
                    IsOpen = false,
                    Entries = null,
                    CreationDate = DateTime.UtcNow.AddDays(-1),
                }
            };

            var parameters = new Dictionary<string, object?>() { { "RegisterId", 1 } };

            _billingPeriodServiceMock.SetupSequence(
                x => x.GetBiilingPeriodsAsync(
                    It.IsAny<int>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(
                new ServiceResponse<IEnumerable<BillingPeriodModel>?>(
                    availableBillingPeriods.AsEnumerable()))
                .ReturnsAsync(
                new ServiceResponse<IEnumerable<BillingPeriodModel>?>(
                    refreshedBillingPeriods));

            _billingPeriodServiceMock.Setup(
                x => x.CloseBillingPeriodAsync(
                    It.IsAny<int>(),
                    It.IsAny<int>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ServiceResponse(true));

            _registerService.Setup(x => x.GetRegister(It.IsAny<int>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ServiceResponse<RegisterModel?>()
                {
                    Result = true,
                    Value = new RegisterModel()
                    {
                        Id = It.IsAny<int>(),
                        Name = It.IsAny<string>(),
                        CreatedDate = It.IsAny<DateTime>(),
                        Description = It.IsAny<string>()
                    }
                });

            await _viewModel.PageParameterSetAsync(parameters);
            await _viewModel.ToggleBillingPeriodAsync();

            Assert.NotNull(_viewModel.AvailableBillingPeriods);
            Assert.NotEmpty(_viewModel.AvailableBillingPeriods);
            Assert.NotNull(_viewModel.SelectedBillingPeriod);
            Assert.False(_viewModel.SelectedBillingPeriod.IsOpen);
            _billingPeriodServiceMock.Verify(
                x => x.CloseBillingPeriodAsync(
                    It.IsAny<int>(),
                    It.IsAny<int>(),
                    It.IsAny<CancellationToken>()),
                Times.Once);
            _billingPeriodServiceMock.Verify(
                x => x.OpenBillingPeriodAsync(
                    It.IsAny<int>(),
                    It.IsAny<int>(),
                    It.IsAny<CancellationToken>()),
                Times.Never);
            _billingPeriodServiceMock.Verify(
                x => x.GetBiilingPeriodsAsync(
                    It.IsAny<int>(),
                    It.IsAny<CancellationToken>()),
                Times.Exactly(2));
        }

        [Fact()]
        public async Task ToggleBillingPeriodAsync_ShouldSetSelectedPeriodStatusAsOpen()
        {
            var availableBillingPeriods = new List<BillingPeriodModel>()
            {
                new BillingPeriodModel()
                {
                    Id = 1,
                    Name = "Test 1",
                    Register = null,
                    IsOpen = false,
                    Entries = null,
                    CreationDate = DateTime.UtcNow,
                },
                new BillingPeriodModel()
                {
                    Id = 2,
                    Name = "Test 2",
                    Register = null,
                    IsOpen = false,
                    Entries = null,
                    CreationDate = DateTime.UtcNow.AddDays(-1),
                }
            };

            var refreshedBillingPeriods = new List<BillingPeriodModel>()
            {
                new BillingPeriodModel()
                {
                    Id = 1,
                    Name = "Test 1",
                    Register = null,
                    IsOpen = false,
                    Entries = null,
                    CreationDate = DateTime.UtcNow,
                },
                new BillingPeriodModel()
                {
                    Id = 2,
                    Name = "Test 2",
                    Register = null,
                    IsOpen = true,
                    Entries = null,
                    CreationDate = DateTime.UtcNow.AddDays(-1),
                }
            };

            var parameters = new Dictionary<string, object?>() { { "RegisterId", 1 } };

            _billingPeriodServiceMock.SetupSequence(
                x => x.GetBiilingPeriodsAsync(
                    It.IsAny<int>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(
                new ServiceResponse<IEnumerable<BillingPeriodModel>?>(
                    availableBillingPeriods.AsEnumerable()))
                .ReturnsAsync(
                new ServiceResponse<IEnumerable<BillingPeriodModel>?>(
                    refreshedBillingPeriods));

            _billingPeriodServiceMock.Setup(
                x => x.OpenBillingPeriodAsync(
                    It.IsAny<int>(),
                    It.IsAny<int>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ServiceResponse(true));

            _registerService.Setup(x => x.GetRegister(It.IsAny<int>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ServiceResponse<RegisterModel?>()
                {
                    Result = true,
                    Value = new RegisterModel()
                    {
                        Id = It.IsAny<int>(),
                        Name = It.IsAny<string>(),
                        CreatedDate = It.IsAny<DateTime>(),
                        Description = It.IsAny<string>()
                    }
                });

            await _viewModel.PageParameterSetAsync(parameters);
            await _viewModel.ToggleBillingPeriodAsync();

            Assert.NotNull(_viewModel.AvailableBillingPeriods);
            Assert.NotEmpty(_viewModel.AvailableBillingPeriods);
            Assert.NotNull(_viewModel.SelectedBillingPeriod);
            Assert.True(_viewModel.SelectedBillingPeriod.IsOpen);
            _billingPeriodServiceMock.Verify(
                x => x.CloseBillingPeriodAsync(
                    It.IsAny<int>(),
                    It.IsAny<int>(),
                    It.IsAny<CancellationToken>()),
                Times.Never);
            _billingPeriodServiceMock.Verify(
                x => x.OpenBillingPeriodAsync(
                    It.IsAny<int>(),
                    It.IsAny<int>(),
                    It.IsAny<CancellationToken>()),
                Times.Once);
            _billingPeriodServiceMock.Verify(
                x => x.GetBiilingPeriodsAsync(
                    It.IsAny<int>(),
                    It.IsAny<CancellationToken>()),
                Times.Exactly(2));
        }

        [Fact()]
        public async Task ToggleBillingPeriodAsync_ShouldDisplayErrorIfOpenPeriodFailed()
        {
            var availableBillingPeriods = new List<BillingPeriodModel>()
            {
                new BillingPeriodModel()
                {
                    Id = 1,
                    Name = "Test 1",
                    Register = null,
                    IsOpen = false,
                    Entries = null,
                    CreationDate = DateTime.UtcNow,
                },
                new BillingPeriodModel()
                {
                    Id = 2,
                    Name = "Test 2",
                    Register = null,
                    IsOpen = false,
                    Entries = null,
                    CreationDate = DateTime.UtcNow.AddDays(-1),
                }
            };

            var parameters = new Dictionary<string, object?>() { { "RegisterId", 1 } };

            _billingPeriodServiceMock.Setup(
                x => x.GetBiilingPeriodsAsync(
                    It.IsAny<int>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(
                new ServiceResponse<IEnumerable<BillingPeriodModel>?>(
                    availableBillingPeriods.AsEnumerable()));

            _billingPeriodServiceMock.Setup(
                x => x.OpenBillingPeriodAsync(
                    It.IsAny<int>(),
                    It.IsAny<int>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ServiceResponse(false));

            _registerService.Setup(x => x.GetRegister(It.IsAny<int>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ServiceResponse<RegisterModel?>()
                {
                    Result = true,
                    Value = new RegisterModel()
                    {
                        Id = It.IsAny<int>(),
                        Name = It.IsAny<string>(),
                        CreatedDate = It.IsAny<DateTime>(),
                        Description = It.IsAny<string>()
                    }
                });

            await _viewModel.PageParameterSetAsync(parameters);
            await _viewModel.ToggleBillingPeriodAsync();

            Assert.NotNull(_viewModel.AvailableBillingPeriods);
            Assert.NotEmpty(_viewModel.AvailableBillingPeriods);
            Assert.NotNull(_viewModel.SelectedBillingPeriod);
            Assert.False(_viewModel.SelectedBillingPeriod.IsOpen);
            _billingPeriodServiceMock.Verify(
                x => x.CloseBillingPeriodAsync(
                    It.IsAny<int>(),
                    It.IsAny<int>(),
                    It.IsAny<CancellationToken>()),
                Times.Never);
            _billingPeriodServiceMock.Verify(
                x => x.OpenBillingPeriodAsync(
                    It.IsAny<int>(),
                    It.IsAny<int>(),
                    It.IsAny<CancellationToken>()),
                Times.Once);
            _billingPeriodServiceMock.Verify(
                x => x.GetBiilingPeriodsAsync(
                    It.IsAny<int>(),
                    It.IsAny<CancellationToken>()),
                Times.Exactly(2));
            _alertServiceMock.Verify(
                x => x.ShowAlertAsync(
                    It.IsAny<string>(),
                    It.IsAny<AlertType>(),
                    It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact()]
        public async Task ToggleBillingPeriodAsync_ShouldDisplayErrorIfClosePeriodFailed()
        {
            var availableBillingPeriods = new List<BillingPeriodModel>()
            {
                new BillingPeriodModel()
                {
                    Id = 1,
                    Name = "Test 1",
                    Register = null,
                    IsOpen = true,
                    Entries = null,
                    CreationDate = DateTime.UtcNow,
                },
                new BillingPeriodModel()
                {
                    Id = 2,
                    Name = "Test 2",
                    Register = null,
                    IsOpen = true,
                    Entries = null,
                    CreationDate = DateTime.UtcNow.AddDays(-1),
                }
            };

            var parameters = new Dictionary<string, object?>() { { "RegisterId", 1 } };

            _billingPeriodServiceMock.Setup(
                x => x.GetBiilingPeriodsAsync(
                    It.IsAny<int>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(
                new ServiceResponse<IEnumerable<BillingPeriodModel>?>(
                    availableBillingPeriods.AsEnumerable()));

            _billingPeriodServiceMock.Setup(
                x => x.CloseBillingPeriodAsync(
                    It.IsAny<int>(),
                    It.IsAny<int>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ServiceResponse(false));

            _registerService.Setup(x => x.GetRegister(It.IsAny<int>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ServiceResponse<RegisterModel?>()
                {
                    Result = true,
                    Value = new RegisterModel()
                    {
                        Id = It.IsAny<int>(),
                        Name = It.IsAny<string>(),
                        CreatedDate = It.IsAny<DateTime>(),
                        Description = It.IsAny<string>()
                    }
                });

            await _viewModel.PageParameterSetAsync(parameters);
            await _viewModel.ToggleBillingPeriodAsync();

            Assert.NotNull(_viewModel.AvailableBillingPeriods);
            Assert.NotEmpty(_viewModel.AvailableBillingPeriods);
            Assert.NotNull(_viewModel.SelectedBillingPeriod);
            Assert.True(_viewModel.SelectedBillingPeriod.IsOpen);
            _billingPeriodServiceMock.Verify(
                x => x.CloseBillingPeriodAsync(
                    It.IsAny<int>(),
                    It.IsAny<int>(),
                    It.IsAny<CancellationToken>()),
                Times.Once);
            _billingPeriodServiceMock.Verify(
                x => x.OpenBillingPeriodAsync(
                    It.IsAny<int>(),
                    It.IsAny<int>(),
                    It.IsAny<CancellationToken>()),
                Times.Never);
            _billingPeriodServiceMock.Verify(
                x => x.GetBiilingPeriodsAsync(
                    It.IsAny<int>(),
                    It.IsAny<CancellationToken>()),
                Times.Exactly(2));
            _alertServiceMock.Verify(
                x => x.ShowAlertAsync(
                    It.IsAny<string>(),
                    It.IsAny<AlertType>(),
                    It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact()]
        public async Task CreateBillingDialogAsync_ShouldReturnErrorMessageIfAllAnyPeriodIsOpen()
        {
            var billingPeriods = new List<BillingPeriodModel>()
            {
                new BillingPeriodModel()
                {
                    Id = 1,
                    Name = "Test 1",
                    Register = null,
                    IsOpen = false,
                    Entries = null,
                    CreationDate = DateTime.UtcNow,
                },
                new BillingPeriodModel()
                {
                    Id = 2,
                    Name = "Test 2",
                    Register = null,
                    IsOpen = true,
                    Entries = null,
                    CreationDate = DateTime.UtcNow.AddDays(-1),
                }
            };

            var parameters = new Dictionary<string, object?>() { { "RegisterId", 1 } };

            _billingPeriodServiceMock.Setup(
                x => x.GetBiilingPeriodsAsync(
                    It.IsAny<int>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(
                new ServiceResponse<IEnumerable<BillingPeriodModel>?>(billingPeriods));
            _registerService.Setup(x => x.GetRegister(It.IsAny<int>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ServiceResponse<RegisterModel?>()
                {
                    Result = true,
                    Value = new RegisterModel()
                    {
                        Id = It.IsAny<int>(),
                        Name = It.IsAny<string>(),
                        CreatedDate = It.IsAny<DateTime>(),
                        Description = It.IsAny<string>()
                    }
                });

            await _viewModel.PageParameterSetAsync(parameters);
            await _viewModel.CreateBillingDialogAsync();

            _alertServiceMock.Verify(
                x => x.ShowAlertAsync(
                    It.IsAny<string>(),
                    It.IsAny<AlertType>(),
                    It.IsAny<CancellationToken>()),
                Times.Once);
            Assert.NotNull(_viewModel.AvailableBillingPeriods);
            Assert.NotEmpty(_viewModel.AvailableBillingPeriods);
            Assert.NotNull(_viewModel.SelectedBillingPeriod);
        }

        [Fact()]
        public async Task CreateBillingDialogAsync_ShouldCreateBillingPeriod()
        {
            var expectedBillingPeriod = new BillingPeriodModel()
            {
                Id = 1,
                Name = "Test",
                IsOpen = true,
                CreationDate = DateTime.Now
            };

            _billingCreateDialogMock.Setup(
                x => x.ShowModalAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedBillingPeriod);

            _billingPeriodServiceMock.Setup(
                x => x.CreateBillingPeriodAsync(
                    It.IsAny<int>(),
                    It.IsAny<BillingPeriodModel>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ServiceResponse(true));

            _billingPeriodServiceMock.Setup(
                x => x.GetBiilingPeriodsAsync(
                    It.IsAny<int>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(
                new ServiceResponse<IEnumerable<BillingPeriodModel>?>(
                    new List<BillingPeriodModel>() { expectedBillingPeriod }));

            await _viewModel.CreateBillingDialogAsync();

            Assert.NotNull(_viewModel.AvailableBillingPeriods);
            Assert.Contains(expectedBillingPeriod, _viewModel.AvailableBillingPeriods);
        }

        [Fact()]
        public async Task PageParameterSetAsync_ShouldSetPageParameters()
        {
            var parameters = new Dictionary<string, object?>() { { "RegisterId", 1 } };

            _billingPeriodServiceMock.Setup(
                x => x.GetBiilingPeriodsAsync(
                    It.IsAny<int>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(
                new ServiceResponse<IEnumerable<BillingPeriodModel>?>(
                    new List<BillingPeriodModel>()
                    {
                        new BillingPeriodModel()
                        {
                            Id = It.IsAny<int>(),
                            Name = It.IsAny<string>(),
                            IsOpen = It.IsAny<bool>(),
                            Entries = It.IsAny<List<EntryModel>>(),
                            Register = It.IsAny<RegisterModel>(),
                            CreationDate = It.IsAny<DateTime>()
                        }
                    }));
            _registerService.Setup(x => x.GetRegister(It.IsAny<int>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ServiceResponse<RegisterModel?>()
                {
                    Result = true,
                    Value = new RegisterModel()
                    {
                        Id = It.IsAny<int>(),
                        Name = It.IsAny<string>(),
                        CreatedDate = It.IsAny<DateTime>(),
                        Description = It.IsAny<string>()
                    }
                });

            await _viewModel.PageParameterSetAsync(parameters);

            Assert.NotNull(_viewModel.AvailableBillingPeriods);
            Assert.NotEmpty(_viewModel.AvailableBillingPeriods);
            Assert.False(_viewModel.IsBusy);
        }

        [Fact()]
        public async Task PageParameterSetAsync_ShouldDisplayErrorIfReadingPeriodsFailed()
        {
            var parameters = new Dictionary<string, object?>() { { "RegisterId", 1 } };
            _billingPeriodServiceMock.Setup(
                x => x.GetBiilingPeriodsAsync(
                    It.IsAny<int>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(
                new ServiceResponse<IEnumerable<BillingPeriodModel>?>(false));

            _registerService.Setup(x => x.GetRegister(It.IsAny<int>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ServiceResponse<RegisterModel?>()
                {
                    Result = true,
                    Value = new RegisterModel()
                    {
                        Id = It.IsAny<int>(),
                        Name = It.IsAny<string>(),
                        CreatedDate = It.IsAny<DateTime>(),
                        Description = It.IsAny<string>()
                    }
                });


            await _viewModel.PageParameterSetAsync(parameters);

            Assert.Null(_viewModel.AvailableBillingPeriods);
            _alertServiceMock.Verify(
                x => x.ShowAlertAsync(
                    It.IsAny<string>(),
                    It.IsAny<AlertType>(),
                    It.IsAny<CancellationToken>()),
                Times.Once);
            Assert.Null(_viewModel.SelectedBillingPeriod);
        }

        [Fact()]
        public async Task PreviousPeriod_ShouldSwitchBillingPeriod()
        {
            var parameters = new Dictionary<string, object?>() { { "RegisterId", 1 } };
            var billingPeriods = new List<BillingPeriodModel>()
            {
                new BillingPeriodModel()
                {
                    Id = 1,
                    Name = "Test 1",
                    IsOpen = It.IsAny<bool>(),
                    CreationDate = It.IsAny<DateTime>(),
                    Entries = It.IsAny<IEnumerable<EntryModel>>(),
                    Register = It.IsAny<RegisterModel>()
                },
                new BillingPeriodModel()
                {
                    Id = 2,
                    Name = "Test 2",
                    IsOpen = It.IsAny<bool>(),
                    CreationDate = It.IsAny<DateTime>(),
                    Entries = It.IsAny<IEnumerable<EntryModel>>(),
                    Register = It.IsAny<RegisterModel>()
                }
            };

            _billingPeriodServiceMock.Setup(
                x => x.GetBiilingPeriodsAsync(
                    It.IsAny<int>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ServiceResponse<IEnumerable<BillingPeriodModel>?>(billingPeriods));
            _registerService.Setup(x => x.GetRegister(It.IsAny<int>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ServiceResponse<RegisterModel?>()
                {
                    Result = true,
                    Value = new RegisterModel()
                    {
                        Id = It.IsAny<int>(),
                        Name = It.IsAny<string>(),
                        CreatedDate = It.IsAny<DateTime>(),
                        Description = It.IsAny<string>()
                    }
                });

            await _viewModel.PageParameterSetAsync(parameters);

            _viewModel.PreviousPeriod();

            Assert.NotNull(_viewModel.AvailableBillingPeriods);
            Assert.NotNull(_viewModel.SelectedBillingPeriod);
            Assert.Equal(billingPeriods[0], _viewModel.SelectedBillingPeriod);
        }

        [Fact()]
        public async Task PreviousPeriod_ShouldNotSwitchPeriodIfBillingPeriodsCollectionIsEmptyOrNull()
        {
            var parameters = new Dictionary<string, object?>() { { "RegisterId", 1 } };
            _billingPeriodServiceMock.Setup(
                x => x.GetBiilingPeriodsAsync(
                    It.IsAny<int>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ServiceResponse<IEnumerable<BillingPeriodModel>?>(false));
            _registerService.Setup(x => x.GetRegister(It.IsAny<int>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ServiceResponse<RegisterModel?>()
                {
                    Result = true,
                    Value = new RegisterModel()
                    {
                        Id = It.IsAny<int>(),
                        Name = It.IsAny<string>(),
                        CreatedDate = It.IsAny<DateTime>(),
                        Description = It.IsAny<string>()
                    }
                });

            await _viewModel.PageParameterSetAsync(parameters);

            _viewModel.PreviousPeriod();

            Assert.Null(_viewModel.SelectedBillingPeriod);
            Assert.Null(_viewModel.AvailableBillingPeriods);
        }

        [Fact()]
        public async Task NextPeriod_ShouldSwitchBillingPeriod()
        {
            var parameters = new Dictionary<string, object?>() { { "RegisterId", 1 } };
            var billingPeriods = new List<BillingPeriodModel>()
            {
                new BillingPeriodModel()
                {
                    Id = 1,
                    Name = "Test 1",
                    IsOpen = It.IsAny<bool>(),
                    CreationDate = It.IsAny<DateTime>(),
                    Entries = It.IsAny<IEnumerable<EntryModel>>(),
                    Register = It.IsAny<RegisterModel>()
                },
                new BillingPeriodModel()
                {
                    Id = 2,
                    Name = "Test 2",
                    IsOpen = It.IsAny<bool>(),
                    CreationDate = It.IsAny<DateTime>(),
                    Entries = It.IsAny<IEnumerable<EntryModel>>(),
                    Register = It.IsAny<RegisterModel>()
                }
            };

            _billingPeriodServiceMock.Setup(
               x => x.GetBiilingPeriodsAsync(
                   It.IsAny<int>(),
                   It.IsAny<CancellationToken>()))
               .ReturnsAsync(new ServiceResponse<IEnumerable<BillingPeriodModel>?>(billingPeriods));
            _registerService.Setup(x => x.GetRegister(It.IsAny<int>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ServiceResponse<RegisterModel?>()
                {
                    Result = true,
                    Value = new RegisterModel()
                    {
                        Id = It.IsAny<int>(),
                        Name = It.IsAny<string>(),
                        CreatedDate = It.IsAny<DateTime>(),
                        Description = It.IsAny<string>()
                    }
                });

            await _viewModel.PageParameterSetAsync(parameters);

            _viewModel.PreviousPeriod();
            _viewModel.NextPeriod();

            Assert.NotNull(_viewModel.AvailableBillingPeriods);
            Assert.NotNull(_viewModel.SelectedBillingPeriod);
            Assert.Equal(billingPeriods[1], _viewModel.SelectedBillingPeriod);
        }

        [Fact()]
        public async Task NextPeriod_ShouldNotSwitchPeriodIfBillingPeriodsCollectionIsEmptyOrNull()
        {
            var parameters = new Dictionary<string, object?>() { { "RegisterId", 1 } };
            _billingPeriodServiceMock.Setup(
                x => x.GetBiilingPeriodsAsync(
                    It.IsAny<int>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ServiceResponse<IEnumerable<BillingPeriodModel>?>(false));
            _registerService.Setup(x => x.GetRegister(It.IsAny<int>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ServiceResponse<RegisterModel?>()
                {
                    Result = true,
                    Value = new RegisterModel()
                    {
                        Id = It.IsAny<int>(),
                        Name = It.IsAny<string>(),
                        CreatedDate = It.IsAny<DateTime>(),
                        Description = It.IsAny<string>()
                    }
                });

            await _viewModel.PageParameterSetAsync(parameters);

            _viewModel.NextPeriod();

            Assert.Null(_viewModel.SelectedBillingPeriod);
            Assert.Null(_viewModel.AvailableBillingPeriods);
        }
    }
}