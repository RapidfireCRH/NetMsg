using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace NetMsg
{
    class _net
    {

        private bool run_once = false;
        UdpClient client;
        int port = 8123;
        
        //error reading
        public Exception error;
        
        //init
        public bool init()
        {
            try
            {
                if (run_once)
                    return true;

                client = new UdpClient();
                client.ExclusiveAddressUse = false;
                client.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
                client.Client.Bind(new IPEndPoint(IPAddress.Any, port));
                run_once = true;
                return true;
            }
            catch(Exception e)
            {
                error = e;
                return false;
            }

        }
        //send
        public bool send(IPAddress ip, string message)
        {
            if (!init())
                return false;
            try
            {
                IPEndPoint endpoint = new IPEndPoint(ip, port);
                client.Send(Encoding.ASCII.GetBytes(message), Encoding.ASCII.GetBytes(message).Length, endpoint);
                return true;
            }
            catch(Exception e)
            {
                error = e;
                return false;
            }
        }
        //receive
        public string receive()
        {
            IPEndPoint endpoint = new IPEndPoint(IPAddress.Any, port);
            return Encoding.ASCII.GetString(client.Receive(ref endpoint));
        }
        //negotiate        
    }
}
