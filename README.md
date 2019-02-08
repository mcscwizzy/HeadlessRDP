# HeadlessRDP
Initiates a remote rdp connection from the command line. 

Opens a remote rdp connection, then leaves a disconnected session open. This is a great utility for TestComplete or other testing software that requires an existing rdp connection for testing.

I have it set to remove the EULA "Legal Entry" entry at logon set by GPO. After it connects, it then updates the GPO so the EULA registry entry comes back, then disconnects the RDP session. 
