Gray Hat C#
===============

This repository contains fully-fleshed out code examples from the book Gray Hat C#. In this book, a wide variety of security oriented tools and libraries will be written using the C# programming language, allowing for cross-platform automation of the most crucial aspects of a security engineer's roles in a modern organization. Many of the topics will also be highly useful for hobbyists and security enthusiasts who are looking to gain more experience with common security concepts and tools with real world examples for both offensive and defensive purposes.


The solution was developed on Linux with MonoDevelop and OS X with Xamarin Studio. However, every line of code should work across all platforms and this should Just Work(tm) in Visual Studio on Windows.

     
The Chapters
====

Chapter 1 - Crash Course
--
In chapter one, we learn the basics of C# object oriented programming with very simple examples. 

<https://msdn.microsoft.com/en-us/library/kx37x362.aspx>

Chapter 2 - Fuzzing and Exploiting XSS and SQL Injection
--
In chapter two, we are introduced to the HTTP library used to communicate with web servers in order to write small HTTP request fuzzers looking for XSS and SQL injection in a variety of different data types. We also write exploits for two types of SQL injection techniques; UNION and boolean-based. 

<https://www.owasp.org/index.php/SQL_Injection>

<https://www.owasp.org/index.php/Cross-site_Scripting_(XSS)>

Chapter 3 - Fuzzing SOAP Endpoints
--
In chapter three, we take the concept of the fuzzers in the previous chapter to the next level, and also introduce the excellent XML libraries available in the standard library. We write a small fuzzer that retrieves and parses a SOAP WSDL in order to automatically generate HTTP requests in order to find potential SQL injections.

<https://en.wikipedia.org/wiki/SOAP>

Chapter 4 - Writing Connect-backs, Binds, and Metasploit Payloads
--
In chapter four, we break from the focus on HTTP and move onto payloads that we can create. We first create couple of simple payloads, one over TCP and one over UDP. Then we learn how to generate x86/x86_64 shellcode in Metasploit to create cross-platform and cross-architecture payloads.

Chapter 5 - Automating Nessus
--
In chapter five, we start back again with HTTP in order to begin automating the Nessus vulnerability scanner. We go over how to create, watch, and report on scans on CIDR ranges programmatically.

<https://www.tenable.com>

Chapter 6 - Automating Nexpose
--
In chapter six, we maintain the focus on tool automation by moving onto automating the Nexpose vulnerability scanner. Nexpose, whose API is also HTTP based, can also achieve automated vulnerability scans and reports and offers a free year license for their Community product, very useful for home enthusiasts.

<https://www.rapid7.com/products/nexpose/>

Chapter 7 - Automating OpenVAS
--
In chapter seven, we conclude the focus on vulnerability scanner automation with OpenVAS, a free and open source vulnerability scanner. OpenVAS has a fundamentally different kind of API than both Nessus and Nexpose, and is also very useful for hobbyists or home enthusiasts.

<http://www.openvas.org>

Chapter 8 - Automating the Cuckoo Sandbox
--
In chapter eight, we move into the incident response area and focus on automating the Cuckoo Sandbox. Using an easy to consume RESTful JSON API, we automate submitting potential malware samples, then reporting on the results.

<http://www.cuckoosandbox.org>

Chapter 9 - Automating sqlmap
--
In chapter nine, we move onto more than just finding potential SQL injections with fuzzers and begin exploiting SQL injections to their fullest extent by automating sqlmap. Using an easy to use JSON API shipped with sqlmap, we first create small tools to submit single URLs. Once done with the introduction, we integrate sqlmap into the SOAP WSDL fuzzer from chapter three, so any potential SQL injection vulnerabilities can be automatically exploited and validated.

<http://sqlmap.org/>

Chapter 10 - Automating ClamAV
--
In chapter ten, we focus on interacting with native, unmanaged libraries. ClamAV, a popular and open source antivirus project, is not written in a .NET language, but we can still interface with its core libraries as well as remotely via a TCP daemon. We cover how to automate ClamAV in both scenarios.

<http://www.clamav.net>

Chapter 11 - Automating Metasploit
--
In chapter eleven, we put the focus back on Metasploit. We learn how to programmatically drive Metasploit via the MSGPACK RPC that is shipped with the core framework in order to exploit and report on shelled hosts.

<https://www.rapid7.com/products/metasploit/>

Chapter 12 - Automating Arachni
--
In chapter twelve, we focus on automating the blackbox web application scanner Arachni, a free and open source project, though dual-licensed. Using both the simpler RESTful HTTP API and the more powerful MSGPACK RPC that is the shipped with the project, we create small tools to automatically scan a URL and report the findings as we scan.

<http://www.arachni-scanner.com>

Chapter 13 - Decompiling and Reversing Managed Assemblies
--
In chapter thirteen, we move into reverse engineering. There are easy to use .NET decompilers for Windows, but not for Mac or Linux, so we write a small one ourselves. We also discuss the useful ```monodis``` tool, which allows granular insight into the inner working of a .NET assembly.

<http://www.mono-project.com/docs/tools+libraries/libraries/Mono.Cecil/>

<http://www.mono-project.com/docs/tools+libraries/tools/monodis/>

Chapter 14 - Reading Offline Windows NT Registry Hives
--
In chapter fourteen, we move into the digital forensics area and focus on registry hives. Going over the binary structure of the Windows registry, we learn how to parse and read offline registry hives, which allows us to easily retrieve the system's boot key, used to encrypt password hash information in the SAM registry hive.

<https://github.com/msuhanov/regf/blob/master/Windows%20registry%20file%20format%20specification.md>

Conclusion
--
In the end, I want the reader to leave having a broad understanding of the potential the C# programming language can have at their home or organization, who may be struggling to enact and follow through with mature vulnerability management or security-oriented SDLCs due to resource constraints.
