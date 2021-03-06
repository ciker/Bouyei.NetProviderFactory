﻿/*-------------------------------------------------------------
 *   auth: bouyei
 *   date: 2017/7/29 13:43:40
 *contact: 453840293@qq.com
 *profile: www.openthinking.cn
 *   guid: 7b7a759e-571f-4486-969a-5306e9dc0f51
---------------------------------------------------------------*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bouyei.NetProviderFactory
{
    using Tcp;
    using Udp;
    public class NetServerProvider : INetServerProvider, IDisposable
    {
        #region variable
        private TcpServerProvider tcpServerProvider = null;
        private UdpServerProvider udpServerProvider = null;
        private int bufferSizeByConnection = 2048;
        private int maxNumberOfConnections = 1024;
        private bool _isDisposed = false;
        #endregion

        #region property
        private OnReceiveHandler _receiveHanlder = null;
        public OnReceiveHandler ReceiveHanlder
        {
            get { return _receiveHanlder; }
            set
            {
                _receiveHanlder = value;
                if (ProviderType.Tcp == NetProviderType)
                {
                    tcpServerProvider.ReceivedCallback = _receiveHanlder;
                }
                else if (ProviderType.Udp == NetProviderType)
                {
                    udpServerProvider.ReceiveCallbackHandler = _receiveHanlder;
                }
            }
        }

        private OnSentHandler _sentHanlder = null;
        public OnSentHandler SentHanlder
        {
            get { return _sentHanlder; }
            set
            {
                _sentHanlder = value;
                if (ProviderType.Tcp == NetProviderType)
                {
                    tcpServerProvider.SentCallback = _sentHanlder;
                }
                else if (ProviderType.Udp == NetProviderType)
                {
                    udpServerProvider.SentCallbackHandler = _sentHanlder;
                }
            }
        }

        private OnAcceptHandler _acceptHanlder = null;
        public OnAcceptHandler AcceptHandler
        {
            get { return _acceptHanlder; }
            set
            {
                _acceptHanlder = value;
                if (ProviderType.Tcp == NetProviderType)
                {
                    tcpServerProvider.AcceptedCallback = _acceptHanlder;
                }
            }
        }

        private OnReceiveOffsetHandler _receiveOffsetHandler = null;
        public OnReceiveOffsetHandler ReceiveOffsetHanlder
        {
            get { return _receiveOffsetHandler; }
            set
            {
                _receiveOffsetHandler = value;
                if (ProviderType.Tcp == NetProviderType)
                {
                    tcpServerProvider.ReceiveOffsetCallback = _receiveOffsetHandler;
                }
                else if (ProviderType.Udp == NetProviderType)
                {
                    udpServerProvider.ReceiveOffsetHanlder = _receiveOffsetHandler;
                }
            }
        }

        private OnDisconnectedHandler _disconnectedHanlder = null;
        public OnDisconnectedHandler DisconnectedHanlder
        {
            get { return _disconnectedHanlder; }
            set
            {
                _disconnectedHanlder = value;
                if (ProviderType.Tcp == NetProviderType)
                {
                    tcpServerProvider.DisconnectedCallback = _disconnectedHanlder;
                }
                else if (ProviderType.Udp == NetProviderType)
                {
                    udpServerProvider.DisconnectedCallbackHandler = _disconnectedHanlder;
                }
            }
        }

        public ProviderType NetProviderType { get; private set; }

        #endregion

        #region constructor
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool isDisposing)
        {
            if (_isDisposed) return;

            if (isDisposing)
            {
                if (tcpServerProvider != null)
                    tcpServerProvider.Dispose();

                if (udpServerProvider != null)
                    udpServerProvider.Dispose();
                _isDisposed = true;
            }
        }

        public NetServerProvider(ProviderType netProviderType = ProviderType.Tcp,
            int bufferSizeByConnection = 4096, int maxNumberOfConnections = 1024)
        {
            this.NetProviderType = netProviderType;
            this.bufferSizeByConnection = bufferSizeByConnection;
            this.maxNumberOfConnections = maxNumberOfConnections;

            if (netProviderType == ProviderType.Tcp)
            {
                tcpServerProvider = new TcpServerProvider(maxNumberOfConnections, bufferSizeByConnection);
            }
            else if (netProviderType == ProviderType.Udp)
            {
                udpServerProvider = new UdpServerProvider();
            }
        }

        public static NetServerProvider CreateNetServerProvider(ProviderType netProviderType = ProviderType.Tcp,
            int bufferSizeByConnection = 4096, int maxNumberOfConnections = 1024)
        {
            return new NetServerProvider(netProviderType, bufferSizeByConnection, maxNumberOfConnections);
        }

        #endregion

        #region public method
        public bool Start(int port, string ip = "0.0.0.0")
        {
            if (NetProviderType == ProviderType.Tcp)
            {
                return tcpServerProvider.Start(port, ip);
            }
            else if (NetProviderType == ProviderType.Udp)
            {
                udpServerProvider.Start(port, bufferSizeByConnection, maxNumberOfConnections);
                return true;
            }
            return false;
        }

        public void Stop()
        {
            if (NetProviderType == ProviderType.Tcp)
            {
                tcpServerProvider.Stop();
            }
            else if (NetProviderType == ProviderType.Udp)
            {
                udpServerProvider.Stop();
            }
        }

        public void Send(SocketToken sToken, byte[] buffer)
        {
            if (NetProviderType == ProviderType.Tcp)
            {
                tcpServerProvider.Send(sToken, buffer);
            }
            else if (NetProviderType == ProviderType.Udp)
            {
                udpServerProvider.Send((System.Net.IPEndPoint)sToken.TokenSocket.RemoteEndPoint,
                    buffer);
            }
        }
        #endregion
    }
}
