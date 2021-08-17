# Using CORE with AWS Lambda

[AWS Lambda](https://aws.amazon.com/lambda/) is a serverless compute service that lets you run code without provisioning or managing servers, creating a workload-aware cluster scaling logic, maintaining event integrations, or managing runtimes. With Lambda, you can run code for virtually any type of application or backend service - all with zero administration. AWS Lambda lets you use Unbound CORE for specific tasks without the need of installing and maintaining any devices, VMs, or Dockers.

To achieve this integration, we use Unbound CORE's *clientless* mode for the Unbound Java JCE Provider. The clientless mode is not dependent on any naive library and can be used in a pure Java Environment.

