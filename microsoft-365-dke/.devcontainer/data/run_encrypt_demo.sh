#!/bin/bash
echo "Running the Java Encrypt demo"
javac -cp "/usr/lib/ekm-java-9-provider-2.0.jar:/usr/lib/ekm-java-utils.jar" /root/demo/com/unbound/samples/encrypt/*.java
java -Djava.library.path=/usr/lib64/:/usr/lib/ekm-java-9-provider-2.0.jar:/usr/lib/ekm-java-utils.jar:/usr/lib64/**/*.jar -cp /root/demo:/usr/lib/ekm-java-9-provider-2.0.jar:/usr/lib/ekm-java-utils.jar com.unbound.samples.encrypt.Main