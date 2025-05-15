# WSL2 설치

## 사전 준비
### Windows Terminal 설치
- [Windows Terminal v1.19.10821.0 다운로드](https://github.com/microsoft/terminal/releases/download/v1.19.10821.0/Microsoft.WindowsTerminal_1.19.10821.0_8wekyb3d8bbwe.msixbundle)


### VSCode 설치
- [VSCode 다운로드](https://code.visualstudio.com/sha/download?build=stable&os=win32-x64-user)
- 확장 도구
  - [Remote Development](https://marketplace.visualstudio.com/items?itemName=ms-vscode-remote.vscode-remote-extensionpack)
  - [Docker](https://marketplace.visualstudio.com/items?itemName=ms-azuretools.vscode-docker)

### WSL2 파일
- [wsl_update_x64.msi 다운로드](https://wslstorestorage.blob.core.windows.net/wslblob/wsl_update_x64.msi)
- [ubuntu-20.04-server-cloudimg-amd64-wsl.rootfs.tar.gz 다운로드](https://cloud-images.ubuntu.com/releases/focal/release/ubuntu-20.04-server-cloudimg-amd64-wsl.rootfs.tar.gz)

<br/>

## WSL2
### WSL2 설치
```shell
# 1. 관리자 권한 Windows Terminal 실행

# 2. 윈도우 버전 확인
#   - OS Version: 10.0.19045 N/A Build 19045        <- x64 시스템의 경우: 버전 1903 이상, 빌드 18362.1049 이상.
cmd.exe /c ver
winver
systeminfo

# 3. Windows 기능 On
# optionalfeatures

# 3.1 Linux용 Windows 하위 시스템 사용
dism.exe /online /enable-feature /featurename:Microsoft-Windows-Subsystem-Linux /all /norestart

# 3.2 Virtual Machine 기능 사용
dism.exe /online /enable-feature /featurename:VirtualMachinePlatform /all /norestart

# 4. Linux 커널 업데이트 패키지 다운로드 & 설치
https://wslstorestorage.blob.core.windows.net/wslblob/wsl_update_x64.msi

# WSL 2를 기본 버전으로 설정
wsl --set-default-version 2
```

### WSL2 Ubuntu 다운로드
- 22.04 LTS jammy jellyfish(https://cloud-images.ubuntu.com/wsl/ 기본 경로 변경)
  - [ubuntu-jammy-wsl-amd64-wsl.rootfs.tar.gz](https://cloud-images.ubuntu.com/wsl/jammy/current/ubuntu-jammy-wsl-amd64-wsl.rootfs.tar.gz)
- 20.04 LTS focal fossa
  - [ubuntu-20.04-server-cloudimg-amd64-wsl.rootfs.tar.gz](https://cloud-images.ubuntu.com/releases/focal/release/ubuntu-20.04-server-cloudimg-amd64-wsl.rootfs.tar.gz)
- 18.04 LTS bionic beaver
  - [ubuntu-18.04-server-cloudimg-amd64-wsl.rootfs.tar.gz](https://cloud-images.ubuntu.com/releases/bionic/release/ubuntu-18.04-server-cloudimg-amd64-wsl.rootfs.tar.gz)

### WSL2 가상화 설치
```powershell
# 설치 준비
#   - 폴더 생성
mkdir -p E:\Workspace\wsl\Ubuntu\20.04\Volume
mkdir -p E:\Workspace\wsl\Ubuntu\20.04\Releases
#   - 파일 복사
#     ubuntu-20.04-server-cloudimg-amd64-wsl.rootfs.tar.gz

# 설치: --import <Distro> <InstallLocation> <FileName> [옵션]
wsl --import ubuntu20.04 `
    E:\Workspace\wsl\Ubuntu\20.04\Volume `
    E:\Workspace\wsl\Ubuntu\20.04\Releases\ubuntu-20.04-server-cloudimg-amd64-wsl.rootfs.tar.gz `
    --version 2

# 제거: --unregister
wsl --unregister ubuntu20.04

# 목록: --list, -l [옵션], --verbose, -v
wsl -l -v
  NAME                   STATE           VERSION
* xyz                    Stopped         2
  ubuntu20.04            Stopped         2

# 실행: --distribution, -d
wsl -d ubuntu20.04

# root 패스워크 변경
passwd

# 종료: --terminate, -t
wsl -t ubuntu20.04
```

### WSL2 계정 로그인
```shell
# 계정 생성
adduser hhko

# 계정 접속 테스트
su - hhko       # hhko 계정 로그인
exit            # hhko 계정 로그 아웃

# 기본 계정 지정: /etc/wsl.conf 파일을 생성한다
tee /etc/wsl.conf <<_EOF
[user]
default=hhko
_EOF

# 기본 계정 테스트
exit
wsl -t ubuntu20.04
wsl -d ubuntu20.04

# 계정 로그인: --user, -u
wsl -d ubuntu20.04            # 기본 계정 로그인
wsl -d ubuntu20.04 -u root    # root 계정 로그인
wsl -d ubuntu20.04 -u hhko    # hhko 계정 로그인
```

### WSL2 기본 가상화 실행
```shell
# 기본: --set-default, -s
wsl -s ubuntu20.04

# 기본 확인
wsl -l -v
  NAME                   STATE           VERSION
* ubuntu20.04            Running         2
  xyz                    Running         2

# 실행
wsl                   # 기본 가상화 실행
wsl -d ubuntu20.04    # ubuntu20.04 가상화 실행
```

### WSL2 파일 공유
```shell
# 윈도우 -> WSL
\\wsl$                # 윈도우 탐색기에서 입력

# 윈도우 <- WSL
mount -l

cd /mnt
ls -al
  c  d  e  wsl  wslg

cd /mnt/c             # 윈도우 C:\ 드라이브
ls -al
```

### WSL2 자원 설정
- `%USERPROFILE%\.wslconfig`
- [Advanced settings configuration in WSL](https://learn.microsoft.com/en-us/windows/wsl/wsl-config)

```
[wsl2]
memory=48GB
processors=8
swap=0
localhostForwarding=true
```

<br/>

## SSH 구성
### SSH 서버: WSL Ubuntu 서버
```shell
# 시스템에 대한 모든 호스트 키를 생성합니다
sudo ssh-keygen -A

# ssh 접속시 암호 입력
sudo vi /etc/ssh/sshd_config
    PasswordAuthentication yes

# 서비스 재시작
sudo service ssh restart

# IP 확인
ip a
    1: lo: <LOOPBACK,UP,LOWER_UP> mtu 65536 qdisc noqueue state UNKNOWN group default qlen 1000
        link/loopback 00:00:00:00:00:00 brd 00:00:00:00:00:00
        inet 127.0.0.1/8 scope host lo
        valid_lft forever preferred_lft forever
        inet6 ::1/128 scope host
        valid_lft forever preferred_lft forever
    2: eth0: <BROADCAST,MULTICAST,UP,LOWER_UP> mtu 1500 qdisc mq state UP group default qlen 1000
        link/ether 00:15:5d:12:89:dd brd ff:ff:ff:ff:ff:ff
        inet 172.19.115.61/20 brd 172.19.127.255 scope global eth0
        valid_lft forever preferred_lft forever
        inet6 fe80::215:5dff:fe12:89dd/64 scope link
        valid_lft forever preferred_lft forever
```

### SSH 클라이언트: 윈도우
```shell
# 1. WSL 서버: Home 디렉토리에 .ssh 폴더를 생성하기(없다면)
ssh hhko@172.19.115.61
mkdir ~/.ssh
ls -al

# 2. 윈도우: SSH 키 생성 : RSA 기반 형식으로 키 만들기
#   - $env:USERPROFILE\.ssh\id_rsa
#   - $env:USERPROFILE\.ssh\id_rsa.pub
ssh-keygen

# 3. 윈도우: SSH Public 키 내용 복사
type $env:USERPROFILE\.ssh\id_rsa.pub | ssh hhko@172.19.115.61 "cat >> .ssh/authorized_keys"

# 4. 윈도우: SSH 접속
ssh hhko@172.19.115.61
```

### VSCode 접속 정보
- `$env:USERPROFILE\.ssh\config`

```
Host "WSL | 172.19.115.61"
    HostName 172.19.115.61
    User hhko

Host *
    StrictHostKeyChecking no
```

<br/>

## Docker Desktop
### Docker Desktop 설치
- https://www.docker.com/products/docker-desktop/

<br/>

## 참고 동영상
- https://www.youtube.com/watch?v=J9pxPVcd-fY&t
- https://www.youtube.com/watch?v=xUNDErAbRiM
