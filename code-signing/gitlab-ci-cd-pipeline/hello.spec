Summary: Hello-world java example
Name: hello
Version: 1.0.0
Release: 1
License: Free
URL: https://gitlab.com/unbound-demo/codesigning-java-rpm/
Group: System
Packager: Yuli
Requires: java
BuildArch: noarch
#BuildRoot: ~/rpmbuild/

%description
Hello-world java example.

%install
rm -rf $RPM_BUILD_ROOT
install -d $RPM_BUILD_ROOT/opt/hello
#install hello.jar $RPM_BUILD_ROOT/opt/hello/hello.jar
cp $OLDPWD/hello.jar $RPM_BUILD_ROOT/opt/hello/hello.jar

%clean
rm -rf $RPM_BUILD_ROOT

%files
%dir /opt/hello
%defattr(-,root,root,-)
/opt/hello/hello.jar

%post
chmod 755 -R /opt/hello
