# ub-ant-signjar-task

The default [signjar][e668d98b]
 task is lacking functionality related to the usage of external cryptograhy providers.
The `ub-ant-signjar-task` extends this functionality to enable integration of this task with EKM.

  [e668d98b]: https://ant.apache.org/manual/Tasks/signjar.html "signjar"

## Prerequisites
* EKM client is installed

* Ant (Version >= 1.93) is installed. 
To install on RH/Centos, use `yum install ant`.
Refer to https://ant.apache.org/bindownload.cgi for further details.


* The `lib:org.apache.ivy.ant` task is installed. 
To install on RH/Centos, use `yum install ivy`.
Refer to http://ant.apache.org/ivy/history/2.2.0/ant.html for further details.

## Build
To build the project run `ant` on the root directory of the project.

## Installation
Copy the generated `.../dist/ub-sign-jar.jar` file to your `ant/lib` directory. Or use full path reference in your project `build.xml`, as follows:
```xml
<taskdef name="UbSignJar" classname="com.unboundtech.UbSignJar">
  <classpath>
    <pathelement location=".../dist/ub-sign-jar.jar"/>
  </classpath>
</taskdef>
```

## Usage
Include the task in your project build.xml to sign jar files with EKM keys, as follows:

```xml
<?xml version="1.0" encoding="UTF-8"?>
<project name="UbSignJar" basedir="." default="ubsign">
   <target name="ubsign">
      <taskdef name="UbSignJar" classname="com.unboundtech.UbSignJar" />
      <UbSignJar jar="test.jar" signedjar="test.jar" partition="part1" alias="key-alias" storepass="" installationDir="C:\Program Files\DyadicSec" />
   </target>
</project>
```
## EKM Specific Task Attributes

All the existing `signjar` options are supported by this new task.
In addition, some EKM specific attributes are included to support using with the EKM cryptograhy provider:

Attribute           | Description                                     | Required | Default
--------------------|-------------------------------------------------|----------|----------------------------------------------------------------------
**partition**       | The EKM partition name to use                   | No       | The first partition on the partition list
**providerPath**    | The path to the EKM Java provider               | No       | The path is computed based on the installation directory
**installationDir** | The directory where the EKM client is installed | No       | `/usr/lib(64)` for Linux or  `C:\Program Files\DyadicSec` for Windows

For the complete set of the standard `signjar` options, refer to [signjar][e668d98b].
