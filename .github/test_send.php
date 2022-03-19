<?php

$request = new HttpRequest();
$request->setUrl('https://onesignal.com/api/v1/notifications');
$request->setMethod(HTTP_METH_POST);

$request->setHeaders(array(
  'postman-token' => 'fb7fae20-936a-1c66-4894-4ea18f3966e8',
  'cache-control' => 'no-cache',
  'content-type' => 'application/json',
  'authorization' => 'Basic MGQ1MzBmODMtNDQ4Mi00NjJlLWJiMzItNjc2NzBjZDAwOTlm'
));

$request->setBody('{
	"include_player_ids": ["555df32a-a66d-11ec-a7a6-62187e8719fe"],
	"app_id": "e8158829-dc25-4f2a-bb4a-7b355338b571",
	"contents":{"en": "Hellooooooooo"},
	"headings": {"en": "hi dude"},
	"include_player_ids": ["555df32a-a66d-11ec-a7a6-62187e8719fe"],
	"ttl": "86400",
	"data": {"AdditionalData":{"negative_note": "negative_note","notification_type":"notificationType"}}
}');

try {
  $response = $request->send();

  echo $response->getBody();
} catch (HttpException $ex) {
  echo $ex;
}
?>