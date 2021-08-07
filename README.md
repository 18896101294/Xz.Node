# XzNode
Personal framework
###### 1. consul 下载地址

```
https://download.tnblog.net/resource/index/836598dc6043401f9e8ab6634b303313
```
###### 2. 两种启动模式
==Client模式：==

这种模式下，所有注册到当前节点的服务会被转发到Server，本身是不持久化这些信息。
Client模式不能脱离Server单独运行，因此无法实现单机模式

==Server模式：==

Consul 的 Server 模式，表明这个 Consul 是个 Server ，这种模式下，功能和 Client
都一样，唯一不同的是，它会把所有的信息持久化的本地，这样遇到故障，信息是可以被保留的。

###### 3. 启动命令

```
consul agent -dev //开发者模式
consul agent -dev -client 0.0.0.0 -ui  //Client模式
```
可以通过[http://localhost:8500/]来访问这个consul，但是，这种模式一般是自己开发的时候用的，因为它不带记忆功能,不能持久化数据，关闭后数据就没有了，也不能与其他consul互通。

```
consul agent -server -bootstrap-expect 1 -data-dir /soft/data/consul -node=consulServer1 -bind=10.267.83.220 -ui -rejoin -config-dir=/soft/config/consul/ -client 0.0.0.0
```
- server ： 定义agent运行在server模式
- bootstrap-expect：在一个datacenter中期望提供的server节点数目，当该值提供的时候，consul一直等到达到指定sever数目的时候才会引导整个集群，该标记不能和bootstrap共用
- bind：该地址用来在集群内部的通讯，集群内的所有节点到地址都必须是可达的，默认是0.0.0.0
- node：节点在集群中的名称，在一个集群中必须是唯一的，默认是该节点的主机名
- ui： 开启consul的界面；
- rejoin：使consul忽略先前的离开，在再次启动后仍旧尝试加入集群中。
- config-dir：配置文件目录，里面所有以.json结尾的文件都会被加载
- client：consul服务侦听地址，这个地址提供HTTP、DNS、RPC等服务，默认是127.0.0.1所以不对外提供服务，如果你要对外提供服务改成0.0.0.
