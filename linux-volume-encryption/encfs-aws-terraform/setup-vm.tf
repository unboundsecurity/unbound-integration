# setup vm

provider "aws" {
  access_key = "${var.access_key}"
  secret_key = "${var.secret_key}"
  region     = "${var.aws_region}"
}

resource "aws_ebs_volume" "testvm_disk" {
  availability_zone = "${var.aws_av_zone}"
  size = 12
  tags = {
    Name = "yuli_testvm_disk"
  }
}

locals {
  vars = {
    //some_address = aws_instance.some.private_ip
    EP_HOST = "${var.ep_host}"
    WGET_EKM_CLIENT = "${var.wget_ekm_client}"
    PARTITION = "${var.partition}"
    ENC_KEYPHASE = "${var.enc_keyphase}"
    RSA_KEY_NAME = "${var.rsa_key_name}"
    ACTIVATION_CODE = "${var.activation_code}"
  }
}

resource "aws_instance" "testvm" {
  //provisioner file {
  //  source      = "${var.ekm_client_rpm}"
  //  destination = "/home/centos/ekm_client.rpm"
  //  connection {
  //    type     = "ssh"
  //    user     = "centos"
  //    host     = "${var.host}"
  //    private_key = "~/.ssh/id_rsa"
  //  }
  //}
  availability_zone = "${var.aws_av_zone}"
  ami = "ami-08ec5ec25b9b7d5c5"
  key_name = "${var.ssh_key_name}"
  instance_type = "t3.small"
  vpc_security_group_ids = [ aws_security_group.testvm_sg.id ]
  user_data = templatefile("userdata.sh", local.vars)
  tags = {
    Name = "yuli_disk_enc"
  }
}

resource "aws_volume_attachment" "ebs_att" {
  device_name = "/dev/xvdh"
  volume_id   = aws_ebs_volume.testvm_disk.id
  instance_id = aws_instance.testvm.id
}

resource "aws_security_group" "testvm_sg" {
  name = "testvm_sg"
  ingress {
    protocol = "tcp"
    from_port = 22
    to_port = 22
    cidr_blocks = [ "0.0.0.0/0" ]
  }
  egress {
    from_port   = 0
    to_port     = 0
    protocol    = "-1"
    cidr_blocks = ["0.0.0.0/0"]
  }
}

output "instance_ips" {
  value = aws_instance.testvm.public_ip
}
