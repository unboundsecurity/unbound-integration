/**
 * Copyright 2018 Dyadic Security Ltd.

 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.

 * You may obtain a copy of the License at
 * http://www.apache.org/licenses/LICENSE-2.0

 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License
 */

package com.unboundtech;

import org.apache.tools.ant.BuildException;
import org.apache.tools.ant.taskdefs.ExecTask;
import org.apache.tools.ant.taskdefs.SignJar;

import java.io.File;
import java.nio.file.Paths;

public class UbSignJar extends SignJar {

    private static final String PROVIDER_JAR_NAME = "ekm-java-provider-2.0.jar";
    private static final String WIN_PROVIDER_FOLDER = "lib";
    private static final String WIN_CLIENT_INSTALLATION_DIR = "C:\\Program Files\\Dyadic\\ekm-client";
    private static final String WIN_SERVER_INSTALLATION_DIR = "C:\\Program Files\\Dyadic\\ekm";

    private static final String UNIX_INSTALLATION_DIR  = "/";
    private static final String UNIX_PROVIDER_FOLDER = "usr/lib";
    private static final String UNIX_64_PROVIDER_FOLDER = "usr/lib64";

    private String partition;
    private String providerPath;
    private String installationDir;

    /**
     * @throws BuildException
     */
    @Override
    public void execute() throws BuildException {

        setStorepass();
        setKeypass();
        setStoretype();

        super.execute();
    }

    /**
     * @return
     */
    @Override
    protected ExecTask createJarSigner() {

        ExecTask cmd = super.createJarSigner();

        setClasspath(cmd);
        setProviderName(cmd);
        setProviderClass(cmd);
        setPartition(cmd);

        return cmd;
    }

    /**
     *
     * @param installationDir
     */
    public void setInstallationDir(String installationDir) {
        this.installationDir = installationDir;
    }

    /**
     * @param providerPath
     */
    public void setProviderPath(String providerPath) {
        this.providerPath = providerPath;
    }

    /**
     * @param partition
     */
    public void setPartition(String partition) {
        this.partition = partition;
    }

    /**
     *
     */
    private void setStorepass() {
        if (storepass == null) {
            setStorepass("null");
        }
    }

    /**
     *
     */
    private void setKeypass() {
        if (keypass == null) {
            setKeypass("null");
        }
    }

    /**
     *
     */
    private void setStoretype() {
        setStoretype("PKCS11");
    }

    /**
     *
     * @param installationDir
     * @param providerFolder
     * @return
     */
    private String resolveProviderFullPath(String installationDir, String providerFolder){
        return resolveProviderFullPath(Paths.get(installationDir).resolve(providerFolder).toAbsolutePath().toString());
    }

    /**
     *
     * @param providerPath
     * @return
     */
    private String resolveProviderFullPath(String providerPath){
        return Paths.get(providerPath).resolve(PROVIDER_JAR_NAME).toAbsolutePath().toString();
    }

    /**
     *
     */
    private String resolveProviderPathOnWindows() {

        String installationDir;

        // try with the client installation dir first
        if (this.installationDir == null) {
            installationDir = WIN_CLIENT_INSTALLATION_DIR;
        } else {
            installationDir = this.installationDir;
        }

        String providerFullPath = resolveProviderFullPath(installationDir, WIN_PROVIDER_FOLDER);

        // try with the server installation dir if file is ot exists
        if (!new File(providerFullPath).exists()) {
            if (this.installationDir == null) {
                installationDir = WIN_SERVER_INSTALLATION_DIR;
                providerFullPath = resolveProviderFullPath(installationDir, WIN_PROVIDER_FOLDER);
            }
        }
        return providerFullPath;
    }

    /**
     *
     */
    private String resolveProviderPathOnUnix() {
        String installationDir;

        if (this.installationDir == null) {
            installationDir = UNIX_INSTALLATION_DIR;
        }
        else {
            installationDir = this.installationDir;
        }

        // try with usr/lib
        String providerFullPath = resolveProviderFullPath(installationDir, UNIX_PROVIDER_FOLDER);

        if(!new File(providerFullPath).exists())
            // try with use/lib64
            providerFullPath = resolveProviderFullPath(installationDir, UNIX_64_PROVIDER_FOLDER);

        return providerFullPath;

    }

    /**
     * @param cmd
     */
    private void setClasspath(ExecTask cmd) {

        String providerFullPath;

        if (providerPath == null) {
            providerFullPath = isWindows() ? resolveProviderPathOnWindows() : resolveProviderPathOnUnix();
        } else {
            providerFullPath = resolveProviderFullPath(providerPath);
        }

        log(String.format("Using provider JAR: %s", providerFullPath));

        if (!new File(providerFullPath).exists())
            throw new BuildException(String.format("Provider jar file (%s) does not exist", providerFullPath));

        String classpath = System.getProperty("java.class.path");

        this.addValue(cmd, "-J-cp");
        this.addValue(cmd, String.format("-J%s%s%s", classpath, File.pathSeparator, providerFullPath));
    }

    /**
     * @param cmd
     */
    private void setProviderName(ExecTask cmd) {
        this.addValue(cmd, "-providerName");
        this.addValue(cmd, "DYADIC");
    }

    /**
     * @param cmd
     */
    private void setProviderClass(ExecTask cmd) {
        this.addValue(cmd, "-providerClass");
        this.addValue(cmd, "com.dyadicsec.provider.DYCryptoProvider");
    }

    /**
     * @param cmd
     */
    private void setPartition(ExecTask cmd) {
        if (partition != null) {
            this.addValue(cmd, "-providerArg");
            this.addValue(cmd, partition);
            log(String.format("Using partition: %s", partition));
        } else {
            log(String.format("Using default partition"));
        }
    }

    /**
     * @return
     */
    public static boolean isWindows() {
        return System.getProperty("os.name").startsWith("Windows");
    }
    
}
