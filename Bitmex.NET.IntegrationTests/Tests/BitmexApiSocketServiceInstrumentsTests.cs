﻿using Bitmex.NET.Dtos;
using Bitmex.NET.Models.Socket;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Bitmex.NET.IntegrationTests.Tests
{
	[TestClass]
	public class BitmexApiSocketServiceInstrumentsTests : BaseBitmexIntegrationTests<IBitmexApiSocketService>
	{
		[TestMethod]
		public void should_connect_and_be_authorized()
		{
			// act
			var result = Sut.Connect();

			// assert
			result.Should().BeTrue();
		}

		[TestMethod]
		public void should_reconnect()
		{
			// act
			var result = Sut.Connect();
			var result2 = Sut.Connect();

			// assert
			(result && result2).Should().BeTrue();
		}

		[TestMethod]
		public void should_subscribe_on_all_instruments()
		{
			// arrange
			var connected = Sut.Connect();

			// act
			IEnumerable<InstrumentDto> dto = null;
			var dataReceived = new ManualResetEvent(false);
			Sut.Subscribe(BitmexApiSubscriptionInfo<IEnumerable<InstrumentDto>>.Create(SubscriptionType.instrument, a =>
			{
				dto = a;
				dataReceived.Set();
			}));
			var received = dataReceived.WaitOne(TimeSpan.FromSeconds(2));

			// assert
			// no exception raised
			connected.Should().BeTrue();
			received.Should().BeTrue();
			dto.Should().NotBeNull();
			dto.Count().Should().BeGreaterThan(0);
		}

		[TestMethod]
		public void should_subscribe_on_all_instruments_with_args()
		{
			// arrange
			var connected = Sut.Connect();

			// act
			IEnumerable<InstrumentDto> dtos = null;
			var dataReceived = new ManualResetEvent(false);
			Sut.Subscribe(BitmexApiSubscriptionInfo<IEnumerable<InstrumentDto>>.Create(SubscriptionType.instrument, a =>
			{
				dtos = a;
				dataReceived.Set();
			}).WithArgs("XBTJPY"));
			var received = dataReceived.WaitOne(TimeSpan.FromSeconds(30));

			// assert
			// no exception raised
			connected.Should().BeTrue();
			received.Should().BeTrue();
			dtos.Should().NotBeNull();
			dtos.Count().Should().BeGreaterThan(0);
			dtos.All(a => a.Symbol == "XBTJPY").Should().BeTrue();
		}
	}
}
