import React, { useEffect, useState } from 'react';
import { createRoot } from 'react-dom/client';
import './style.css';
const API = import.meta.env.VITE_API_URL || 'https://localhost:7443';
function App() {
 const [token, setToken] = useState(sessionStorage.getItem('hotel_token') || '');
 const [me, setMe] = useState(null), [login, setLogin] = useState('demo_user1'), [password, setPassword] = useState('');
 const [mode, setMode] = useState('login'), [busy, setBusy] = useState(false);
 const [products, setProducts] = useState([]), [reservations, setReservations] = useState([]);
 const [productId, setProductId] = useState(''), [quantity, setQuantity] = useState(1), [checkId, setCheckId] = useState('');
 const [events, setEvents] = useState([]), [notice, setNotice] = useState('');
 const log = (method,path,status,message) => setEvents(prev => [{time:new Date().toLocaleTimeString(),method,path,status,message},...prev].slice(0,20));
 async function request(path,{method='GET',body,access=token}={}) {
   let response;
   try { response = await fetch(API+path,{method,headers:{...(body ? {'Content-Type':'application/json'}:{}),...(access ? {Authorization:'Bearer '+access}:{})},body:body ? JSON.stringify(body):undefined}); }
   catch {
     const message = 'Нет доступа к API: возможна блокировка CORS, ошибка сертификата или недоступен сервер. Проверьте Network и Console в DevTools.';
     log(method,path,'СЕТЬ',message); throw new Error(message);
   }
   const text = await response.text(); let data; try {data=JSON.parse(text);} catch {data=text;}
   const message = response.ok ? 'Запрос выполнен' : data?.error || data?.title || 'Запрос отклонён';
   log(method,path,response.status,message);
   if(!response.ok) {
     if(response.status===401 && access){setToken('');setMe(null);sessionStorage.removeItem('hotel_token');}
     throw new Error(response.status+': '+message);
   }
   return data;
 }
 async function run(action) {setBusy(true);setNotice('');try{await action();}catch(e){setNotice(e.message);}finally{setBusy(false);}}
 async function refresh(access=token) {
   const items=await request('/api/productreservations',{access});setReservations(items);
 }
 useEffect(()=>{run(async()=>{
   const list=await request('/api/products?pageSize=100',{access:''});setProducts(list);
   setProductId(String(list[0]?.idProduct||''));
   if(token){setMe(await request('/api/auth/me'));await refresh();}
 });},[]);
 function logout(){setToken('');setMe(null);setReservations([]);sessionStorage.removeItem('hotel_token');setNotice('Вы вышли из аккаунта.');}
 async function authenticate(e){e.preventDefault();await run(async()=>{
   const data=await request('/api/auth/'+mode,{method:'POST',body:{login,password},access:''});
   sessionStorage.setItem('hotel_token',data.accessToken);setToken(data.accessToken);setPassword('');
   setMe(await request('/api/auth/me',{access:data.accessToken}));await refresh(data.accessToken);
   setNotice(mode==='register'?'Аккаунт создан. Вы вошли в систему.':'Вход выполнен.');
 });}
 return <div className="app">
 <aside><a className="brand" href="/">H<span>HOTEL<small>Личный кабинет</small></span></a><div className="nav active">◈ &nbsp; Бронирования</div><div className="aside-bottom"><span className="dot"/> Учебный проект<small>Практическая работа №11<br/>JWT · Policies · CORS · Rate limit</small></div></aside>
 <main><header><div><div className="eyebrow">СЕРВИС БРОНИРОВАНИЯ</div><h1>Ваши бронирования</h1><p>Выбирайте товары и управляйте своими заявками.</p></div><span className="badge">{me ? me.role?.roleName+' / '+me.login : 'Гостевой доступ'}</span></header>
 <div className="connection"><span className="dot"/> Клиент: <b>{location.origin}</b><span>→</span> API: <b>{API}</b></div>
 {notice && <div className="notice" role="status">{notice}</div>}
 <div className="grid">
 <section className="card auth"><div className="eyebrow">01 / АККАУНТ</div><h2>{me?'Вы в системе':'Добро пожаловать'}</h2>
 {me ? <><div className="avatar">{me.login[0].toUpperCase()}</div><h3>{me.login}</h3><p>ID {me.idUser} · {me.role?.roleName}</p><p className="muted">Токен сохранён на время этой вкладки. Защищённые запросы отправляются с заголовком Bearer.</p><button disabled={busy} onClick={()=>run(async()=>{setMe(await request('/api/auth/me'));setNotice('Защищённый профиль получен.');})}>Проверить мой профиль</button><button className="secondary" onClick={logout}>Выйти</button></> :
 <><div className="tabs"><button className={mode==='login'?'selected':''} onClick={()=>setMode('login')}>Вход</button><button className={mode==='register'?'selected':''} onClick={()=>setMode('register')}>Регистрация</button></div><form onSubmit={authenticate}><label>Логин<input required minLength={mode==='register'?3:1} maxLength={50} value={login} autoComplete="username" onChange={e=>setLogin(e.target.value)}/></label><label>Пароль<input type="password" required minLength={mode==='register'?8:1} maxLength={128} value={password} autoComplete={mode==='register'?'new-password':'current-password'} onChange={e=>setPassword(e.target.value)}/></label><button disabled={busy}>{busy?'Подождите…':mode==='login'?'Войти в аккаунт':'Создать аккаунт'}</button></form><p className="muted">Регистрация создаёт пользователя с ролью User. Пароль: от 8 символов.</p></>}
 </section>
 <section className="card"><div className="eyebrow">02 / НОВАЯ ЗАЯВКА</div><h2>Забронировать товар</h2><p className="muted">Владелец бронирования определяется автоматически по вашему аккаунту.</p><form onSubmit={e=>{e.preventDefault();run(async()=>{const item=await request('/api/productreservations',{method:'POST',body:{productId:Number(productId),quantity:Number(quantity)}});setCheckId(String(item.idProductReservation));await refresh();setNotice('Бронирование №'+item.idProductReservation+' создано.');});}}>
 <label>Товар<select value={productId} onChange={e=>setProductId(e.target.value)} required><option value="" disabled>Выберите товар</option>{products.map(p=><option key={p.idProduct} value={p.idProduct}>{p.productName} · {p.price} ₽</option>)}</select></label><label>Количество<input type="number" min="1" max="100" value={quantity} onChange={e=>setQuantity(e.target.value)} required/></label><button disabled={!me||busy||!productId}>Создать бронирование <span>↗</span></button></form>{!me&&<p className="muted">Войдите, чтобы создать бронирование.</p>}
 </section>
 </div>
 <section className="card list"><div className="section-head"><div><div className="eyebrow">03 / МОИ ЗАЯВКИ</div><h2>{me?.role?.roleName==='Admin'?'Все бронирования':'История бронирований'}</h2></div><button className="secondary compact" disabled={busy||!me} onClick={()=>run(()=>refresh())}>Обновить</button></div>
 {reservations.length ? <div className="table-wrap"><table><thead><tr><th>Номер</th><th>Товар</th><th>Владелец</th><th>Количество</th><th>Статус</th><th></th></tr></thead><tbody>{reservations.map(r=><tr key={r.idProductReservation}><td>#{r.idProductReservation}</td><td>{r.product?.productName||r.productId}</td><td>#{r.userId}</td><td>{r.quantity}</td><td><span className={'status '+(r.cancelledAt?'cancelled':'')}>{r.cancelledAt?'Отменено':'Активно'}</span></td><td><button className="link" disabled={busy||!!r.cancelledAt} onClick={()=>run(async()=>{await request('/api/productreservations/'+r.idProductReservation+'/cancel',{method:'POST'});await refresh();setNotice('Бронирование отменено.');})}>Отменить</button></td></tr>)}</tbody></table></div>:
 <div className="empty"><span>◇</span><h3>{me?'Бронирований пока нет':'Здесь появятся ваши бронирования'}</h3><p>{me?'Создайте первую заявку с помощью формы выше.':'Войдите в аккаунт, чтобы увидеть свои заявки.'}</p></div>}
 </section>
 <section className="card"><div className="eyebrow">04 / ПРОВЕРКА ДОСТУПА</div><h2>Проверить ограничения</h2><p className="muted">Сервер проверяет права при каждом запросе. Чужой ресурс — 403, запрос без токена — 401.</p><div className="checks"><label>ID бронирования<input type="number" min="1" value={checkId} onChange={e=>setCheckId(e.target.value)} placeholder="Например, 1"/></label><button className="secondary" disabled={busy||!checkId} onClick={()=>run(async()=>{const r=await request('/api/productreservations/'+checkId);setNotice('Доступ разрешён: бронирование №'+r.idProductReservation);})}>Прочитать по ID</button><button className="secondary" disabled={busy||!checkId} onClick={()=>run(async()=>{await request('/api/productreservations/'+checkId+'/cancel',{method:'POST'});if(me)await refresh();setNotice('Отмена выполнена.');})}>Отменить по ID</button></div><div className="actions"><button className="secondary" disabled={busy} onClick={()=>run(async()=>{await request('/api/users');setNotice('Административный доступ разрешён.');})}>Действие Admin</button><button className="secondary" disabled={busy} onClick={()=>run(()=>request('/api/auth/me',{access:''}))}>Запрос без токена</button><button className="secondary" disabled={busy} onClick={()=>run(async()=>{for(let i=0;i<6;i++){try{await request('/api/auth/login',{method:'POST',body:{login:'rate_limit_probe',password:'invalid-password'},access:''});}catch{}}setNotice('Шесть запросов выполнены. Проверьте статус 429 в журнале.');})}>Проверить лимит входа</button></div></section>
 <section className="card journal"><div className="section-head"><div><div className="eyebrow">05 / ЖУРНАЛ ЗАПРОСОВ</div><h2>Что ответил сервер</h2></div><button className="link" onClick={()=>setEvents([])}>Очистить</button></div><div aria-live="polite">{events.length ? events.map((ev,i)=><div className="event" key={i}><time>{ev.time}</time><span className={'code '+(Number(ev.status)>=400||ev.status==='СЕТЬ'?'error':'')}>{ev.status}</span><div><code>{ev.method} {ev.path}</code><p>{ev.message}</p></div></div>):<p className="muted">Выполните запрос — его результат появится здесь.</p>}</div><p className="muted">Для проверки CORS откройте второй клиент на localhost:5174. Подробности блокировки доступны в DevTools. Токены и пароли в журнал не выводятся.</p></section>
 <footer>HOTEL / Практическая №11<span>React + ASP.NET Core</span></footer>
 </main></div>
}
createRoot(document.getElementById('root')).render(<App/>);

