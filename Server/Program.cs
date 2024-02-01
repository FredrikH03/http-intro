using System.Net;
using System.Text;

bool listen = true;

Console.CancelKeyPress += delegate(object sender, ConsoleCancelEventArgs e) 
{ 
  Console.WriteLine("interrupting cancel event");
  e.Cancel = true;
  listen = false;
};

int port = 3000;
HttpListener listener = new();

listener.Prefixes.Add($"http://127.0.0.1:{port}/");
listener.Start();
Console.WriteLine($"Server listening on port: {port}");

listener.BeginGetContext(new AsyncCallback(Router), listener);
while (listen) { }

listener.Stop();
Console.WriteLine("server stopped");



void Router(IAsyncResult result)
{
  if (result.AsyncState is HttpListener listener)
  {
    HttpListenerContext context = listener.EndGetContext(result);

    HttpListenerRequest request = context.Request;
    string path = request.Url?.AbsolutePath ?? "/";
    
    
    if(request.HttpMethod == "GET")
    {
      Console.WriteLine("requesting resource at: " + request.Url?.AbsolutePath);
    } 
    else if (request.HttpMethod == "POST")
    {
      
      StreamReader? reader = new(request.InputStream, request.ContentEncoding);
      string data = reader.ReadToEnd();
      Console.WriteLine($"received data: {data}");
      
      if (request.Url?.AbsolutePath == "/users")
      {
        UploadUser(data);
        
      }
      else
      {
        
      }
    }
    //Console.WriteLine($"{request.Url}, {request.HttpMethod}");
    HttpListenerResponse response = context.Response;
    byte[] buffer = null; 
    switch (request.Url.ToString())
    {
      case "http://127.0.0.1:3000/":
        response.StatusCode = (int)HttpStatusCode.OK; // = 200
        response.ContentType = "text/plain";
        buffer = Encoding.UTF8.GetBytes("localhost");
        response.OutputStream.Write(buffer, 0, buffer.Length);
        break;
      case "http://127.0.0.1:3000/users":
        response.StatusCode = (int)HttpStatusCode.OK; // = 200
        response.ContentType = "text/plain";
        buffer = Encoding.UTF8.GetBytes("users in localhost \nfanduill"); 
        response.OutputStream.Write(buffer, 0, buffer.Length);
        break;

    }
    
   
    
    //response.StatusCode = (int)HttpStatusCode.OK; // = 200
    //response.ContentType = "text/plain";
    //byte[] buffer = Encoding.UTF8.GetBytes("Hello");
    //response.OutputStream.Write(buffer, 0, buffer.Length);
    response.OutputStream.Close();
    
    listener.BeginGetContext(new AsyncCallback(Router), listener);
  }
}

void UploadUser(string data)
{
  string[] user = data.Split(",");
  string name = user[0];
  string pass = user[1];
  

  //insert db logic

  Console.WriteLine($"created new user with name: {name} and password {pass}");
}
