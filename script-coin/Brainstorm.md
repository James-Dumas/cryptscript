Main Thread (foreground processor)
	>Handles UI and related processes 
	>Scripts Involved; `Commands.cs`, `Dictionaries.cs`, `Encryptor.cs`, `Miner.cs`(mostly), `Program.cs`, `WalletGen.cs`
	>These things need to happen when the user wants them to and display something on the screen
	>Main script is `Program.cs` and this calls `Commands.cs` which calls the actual program

Some programs only run when called; `Dictionaries.cs`, Encryptor.cs`, WalletGen.cs`
	>`Dictionaries.cs` has lists of commands, jokes, and other important things that are rather bulky and in list form.
	>`Encryptor.cs`
		>If key from user then check to see if the private address hashes to the same thing as the checksum on public address
		>If key from `WalletGen.cs` then don't do anything
			>**KEEP LOOKING FOR SOME WAY TO MAKE STRINGS SHORTER**	
	>`WalletGen.cs` generates a valid pair of private and public addresses 

Standby Threads (becomes foreground when called)
	>`Miner.cs` stands by until user starts mining and then it becomes the primary thread
		>Creates n threads, where n is the (number of logical cores - 1) on the users system

Background Thread (runs the entire time the `Program.cs` is running)
	>`Blockchain.cs` controlls the user node according to the rest of the block chain
		>`Blockchain.Difficulty()` controls the difficulty rating (how many 0's for good hash) based on how long the last five block were apart
			>If (aveDifference < 55 seconds) then (difficulty = difficulty + "0")
			>If (aveDifference > 65 seconds) then (difficulty = difficulty - "0")
				>If ((difficulty - "0") = string.Empty) then (difficulty = "0")
		>`Blockchain.Reward()` controls amount of Script-Coin miner gets for finding good hash value
			>Starts at 50/block
			>75% at every 211,680 blocks
				>After first round
			>26,460,050 coins total
			>17,781,121 blocks total
			>0.00248 SCT per transaction added to block reward
		>`Blockchain.Contact()`	finds other active nodes on startup
			>On first start up check DNS server for IP addresses of other nodes
				>If the seeds from DNS server bad then fall back to hard coded known good nodes
			>Send DNS request to other known nodes to get reply and if good then connection is verified
				>**THINK ABOUT REQUEST/REPLY TERMS** (ping/pong(?))		
			>On concurrent start ups look at nodes.txt
				>If (time since last start >= 864000) then clear list and fallback to DNS server
				>If (list of IP addresses > 100) then request more from other addresses
				>If (list of IP addresses < 100) then truncate list
				>Iterate through list until (goodNodes >= 10)
		>`Blockchain.Send()` sends information into the blockchain to get verified
		>`Blockchain.Recieve()` gets information from the blockchain