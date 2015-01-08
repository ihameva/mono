﻿using System;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using NUnit.Framework;
using NUnit.Core;
using Mono.Security.Protocol.NewTls;
using Mono.Security.Protocol.NewTls.Cipher;

namespace Mono.Security.Instrumentation.Tests
{
	using Framework;
	using Resources;

	class SimpleConnectionTest : ConnectionTest
	{
		public SimpleConnectionTest (TestConfiguration config, ClientAndServerFactory factory)
			: base (config, factory)
		{
		}

		[Test]
		[Category ("Simple")]
		public async void Simple ()
		{
			await Run (new ClientAndServerParameters {
				VerifyPeerCertificate = false
			});
		}

		[Test]
		[Category ("Simple")]
		public async void Simple_VerifyCertificate ()
		{
			await Run (new ClientAndServerParameters {
				ServerCertificate = ResourceManager.ServerCertificateFromCA,
				VerifyPeerCertificate = true, TrustedCA = ResourceManager.LocalCACertificate
			});
		}

		[Test]
		[Category ("Simple")]
		public async void Simple_AskForCertificate ()
		{
			await Run (new ClientAndServerParameters {
				ServerCertificate = ResourceManager.ServerCertificateFromCA,
				VerifyPeerCertificate = true, TrustedCA = ResourceManager.LocalCACertificate,
				AskForClientCertificate = true
			});
		}

		[Test]
		[Category ("Simple")]
		public async void Simple_RequireCertificate ()
		{
			await Run (new ClientAndServerParameters {
				ServerCertificate = ResourceManager.ServerCertificateFromCA,
				VerifyPeerCertificate = true, TrustedCA = ResourceManager.LocalCACertificate,
				RequireClientCertificate = true, ClientCertificate = ResourceManager.MonkeyCertificate
			});
		}

		[Test]
		[Category ("Simple")]
		public async void CheckCipherSuite ()
		{
			if (!Factory.HasConnectionInfo)
				throw new IgnoreException ("Current implementation does not support ConnectionInfo.");

			var expectedCipher = CipherSuiteCode.TLS_DHE_RSA_WITH_AES_256_GCM_SHA384;

			await Run (new ClientAndServerParameters {
				VerifyPeerCertificate = false
			}, connection => {
				var connectionInfo = connection.Server.GetConnectionInfo ();
				Assert.That (connectionInfo, Is.Not.Null, "#1");
				Assert.That (connectionInfo.CipherCode, Is.EqualTo (expectedCipher), "#2");
			});
		}

		IEnumerable<CipherSuiteCode> GetAllCipherCodes ()
		{
			// Galois-Counter Cipher Suites.
			yield return CipherSuiteCode.TLS_DHE_RSA_WITH_AES_256_GCM_SHA384;
			yield return CipherSuiteCode.TLS_DHE_RSA_WITH_AES_128_GCM_SHA256;

			// Galois-Counter with Legacy RSA Key Exchange.
			yield return CipherSuiteCode.TLS_RSA_WITH_AES_128_GCM_SHA256;
			yield return CipherSuiteCode.TLS_RSA_WITH_AES_256_GCM_SHA384;

			// Diffie-Hellman Cipher Suites
			yield return CipherSuiteCode.TLS_DHE_RSA_WITH_AES_256_CBC_SHA256;
			yield return CipherSuiteCode.TLS_DHE_RSA_WITH_AES_128_CBC_SHA256;
			yield return CipherSuiteCode.TLS_DHE_RSA_WITH_AES_256_CBC_SHA;
			yield return CipherSuiteCode.TLS_DHE_RSA_WITH_AES_128_CBC_SHA;

			// Legacy AES Cipher Suites
			yield return CipherSuiteCode.TLS_RSA_WITH_AES_256_CBC_SHA256;
			yield return CipherSuiteCode.TLS_RSA_WITH_AES_128_CBC_SHA256;
			yield return CipherSuiteCode.TLS_RSA_WITH_AES_256_CBC_SHA;
			yield return CipherSuiteCode.TLS_RSA_WITH_AES_128_CBC_SHA;
		}

		[Test]
		[Category ("Simple")]
		[TestCaseSource ("GetAllCipherCodes")]
		public async void TestAllCiphers (CipherSuiteCode code)
		{
			if (!Factory.CanSelectCiphers)
				throw new IgnoreException ("Current implementation does not let us select ciphers.");

			var requestedCiphers = new CipherSuiteCollection (TlsProtocolCode.Tls12, code);

			await Run (new ClientAndServerParameters {
				VerifyPeerCertificate = false,
				ClientCiphers = requestedCiphers
			}, connection => {
				var connectionInfo = connection.Server.GetConnectionInfo ();
				Assert.That (connectionInfo, Is.Not.Null, "#1");
				Assert.That (connectionInfo.CipherCode, Is.EqualTo (code), "#2");
			});
		}

		async Task Run (ClientAndServerParameters parameters, Action<ClientAndServer> action = null)
		{
			try {
				if (Configuration.EnableDebugging)
					parameters.EnableDebugging = true;
				using (var connection = (ClientAndServer)await Factory.Start (parameters)) {
					if (action != null)
						action (connection);
					var handler = ConnectionHandlerFactory.HandshakeAndDone.Create (connection);
					await handler.Run ();
				}
			} catch (Exception ex) {
				DebugHelper.WriteLine ("ERROR: {0} {1}", ex.GetType (), ex);
				throw;
			}
		}
	}
}
