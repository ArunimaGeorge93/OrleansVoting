from locust import HttpUser, task

class HelloWorldUser(HttpUser):
    def on_start(self):
        """ on_start is called when a Locust start before any task is scheduled """
        self.client.verify = False
    @task
    def vote(self):
        self.client.get("/1")

    @task
    def load_poll(self):
        self.client.get("/")
    
