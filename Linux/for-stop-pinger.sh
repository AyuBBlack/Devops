for i in 'ps aux | grep pinger | head -1 | awk {'print $2'}';do
	kill -9 $i;
done
