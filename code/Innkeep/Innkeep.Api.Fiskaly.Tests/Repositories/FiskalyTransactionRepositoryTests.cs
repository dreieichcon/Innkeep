﻿using Innkeep.Api.Auth;
using Innkeep.Api.Enum.Fiskaly.Transaction;
using Innkeep.Api.Fiskaly.Repositories.Auth;
using Innkeep.Api.Fiskaly.Repositories.Transaction;
using Innkeep.Api.Fiskaly.Tests.Data;
using Innkeep.Api.Fiskaly.Tests.Mock;
using Innkeep.Api.Models.Fiskaly.Objects.Transaction;
using Innkeep.Api.Models.Fiskaly.Request.Transaction;

namespace Innkeep.Api.Fiskaly.Tests.Repositories;

[TestClass]
public class FiskalyTransactionRepositoryTests
{
	private FiskalyAuthenticationRepository _authenticationRepository = null!;

	private IFiskalyAuthenticationService _authenticationService;

	private readonly ITestAuth _testAuth = new TestAuth();

	private FiskalyTransactionRepository _transactionRepository;

	[TestInitialize]
	public void Initialize()
	{
		_authenticationRepository = new FiskalyAuthenticationRepository();
		_authenticationService = new FiskalyAuthenticationServiceMock(_authenticationRepository);
		_transactionRepository = new FiskalyTransactionRepository(_authenticationService);
	}

	[TestMethod]
	public async Task CreateTransaction_ReturnsCompletedTransaction()
	{
		var transactionId = Guid.NewGuid().ToString();

		var transaction = await _transactionRepository.StartTransaction(
			_testAuth.FiskalyTestTssId,
			transactionId,
			_testAuth.FiskalyTestClientId
		);

		Assert.IsNotNull(transaction);

		var updateRequest = new FiskalyTransactionUpdateRequest
		{
			ClientId = _testAuth.FiskalyTestClientId,
			TssId = _testAuth.FiskalyTestTssId,
			Schema = new FiskalyTransactionSchema
			{
				StandardV1 = new FiskalySchemaStandardV1
				{
					Receipt = new FiskalyReceipt
					{
						ReceiptType = ReceiptType.Receipt,
						AmountsPerVatRate = new List<FiskalyAmountPerVatRate>
						{
							new()
							{
								Amount = 0,
								VatRate = VatRate.Null,
							},
						},
					},
				},
			},
			State = TransactionState.Finished,
			TransactionId = transactionId,
			TransactionRevision = 2,
		};

		var update = await _transactionRepository.UpdateTransaction(updateRequest);

		Assert.IsNotNull(update);
	}
}