﻿namespace Chiota.Messenger.Tests.Usecase
{
  using System.Collections.Generic;
  using System.Threading.Tasks;

  using Chiota.Messenger.Entity;
  using Chiota.Messenger.Tests.Encryption;
  using Chiota.Messenger.Tests.Repository;
  using Chiota.Messenger.Tests.Service;
  using Chiota.Messenger.Usecase;
  using Chiota.Messenger.Usecase.GetMessages;
  using Chiota.Messenger.Usecase.SendMessage;

  using Microsoft.VisualStudio.TestTools.UnitTesting;

  using Tangle.Net.Entity;

  [TestClass]
  public class GetMessagesInteractorTest
  {
    [TestMethod]
    public async Task TestMessengerThrowExceptionShouldReturnErrorCode()
    {
      var interactor = new GetMessagesInteractor(new ExceptionMessenger(), new EncryptionStub(), new EncryptionStub());
      var result = await interactor.ExecuteAsync(
        new GetMessagesRequest
          {
            ChatAddress = new Address(Hash.Empty.Value),
            ChatKeyPair = InMemoryContactRepository.NtruKeyPair
          });

      Assert.AreEqual(ResponseCode.MessengerException, result.Code);
    }

    [TestMethod]
    public async Task TestInvalidMessagesShouldBeIgnored()
    {
      var messenger = new InMemoryMessenger();
      messenger.SentMessages.Add(new Message(new TryteString("GHAFSGHAFSGHFASAAS"), new Address(Hash.Empty.Value)));

      var interactor = new GetMessagesInteractor(messenger, new EncryptionStub(), new EncryptionStub());
      var result = await interactor.ExecuteAsync(
                     new GetMessagesRequest
                       {
                         ChatAddress = new Address(Hash.Empty.Value),
                         ChatKeyPair = InMemoryContactRepository.NtruKeyPair
                       });

      Assert.AreEqual(ResponseCode.Success, result.Code);
    }

    [TestMethod]
    public async Task TestValidMessageCanBeParsedCorrectly()
    {
      Assert.Inconclusive("TODO: Encryption Stub");
      var messenger = new InMemoryMessenger();
      var sendMessageInteractor = new SendMessageInteractor(messenger, new EncryptionStub(), new EncryptionStub());
      await sendMessageInteractor.ExecuteAsync(
        new SendMessageRequest
          {
            ChatAddress = new Address(Hash.Empty.Value),
            ChatKeyPair = InMemoryContactRepository.NtruKeyPair,
            Message = "Hallo",
            UserPublicKeyAddress = new Address(Hash.Empty.Value)
          });

      var interactor = new GetMessagesInteractor(messenger, new EncryptionStub(), new EncryptionStub());
      var result = await interactor.ExecuteAsync(
                     new GetMessagesRequest
                       {
                         ChatAddress = new Address(Hash.Empty.Value),
                         ChatKeyPair = InMemoryContactRepository.NtruKeyPair
                       });

      Assert.AreEqual(ResponseCode.Success, result.Code);
      Assert.AreEqual(1, result.Messages.Count);
      Assert.AreEqual("Hallo", result.Messages[0].Message);
    }
  }
}