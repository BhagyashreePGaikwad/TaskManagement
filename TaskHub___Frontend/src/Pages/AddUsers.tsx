import React, { useEffect, useState } from "react";
import axios from "axios";

import "react-toastify/dist/ReactToastify.css";
export default function AddUsers() {
  const [user, setuser] = useState({
    Name: "",
    Role: "1",
    Email: "",
    Username: "",
    Password: "",
  });
  const [role, setrole] = useState([]);

  const handleChange = (_e: any) => {
    _e.preventDefault();
    setuser({ ...user, [_e.target.name]: _e.target.value });
  };
  const getrole = () => {
    axios
      .get( `${import.meta.env.VITE_REACT_APP_BACKEND_URL}/Role`)
      .then((r) => {
        setrole(r.data);
        console.log(r.data);
      })
      .catch((_error) => {
        alert("error in loading roles");
      });
  };

  useEffect(() => {
    getrole();
  }, []);

  const adduser = (_e: any) => {
    _e.preventDefault();
    setuser({ ...user, [_e.target.name]: _e.target.value });
    const t = {
      name: user.Name,
      roleId: user.Role,
      email: user.Email,
      username: user.Username,
      password: user.Password,
    };
    console.log(t);
    axios
      .post( `${import.meta.env.VITE_REACT_APP_BACKEND_URL}/User`, t)
      .then((_result) => {
        console.log(t);
      })
      .catch((_error) => {
        alert("Cant add user");
      });
    setuser({
      Name: "",
      Role: "",
      Email: "",
      Username: "",
      Password: "",
    });
  };

  return (
    <>
      <form onSubmit={adduser}>
        Name:
        <input
          type="text"
          name="Name"
          onChange={handleChange}
          value={user.Name}
        />
        Role:
        <select name="Role" onChange={handleChange} value={user.Role}>
          {role && role.length > 0
            ? role.map((item: any, _index: any) => {
                return <option value={item.id}>{item.roleName}</option>;
              })
            : "Loading"}
        </select>
        Email:
        <input
          type="email"
          name="Email"
          onChange={handleChange}
          value={user.Email}
        />
        Username:
        <input
          type="text"
          name="Username"
          onChange={handleChange}
          value={user.Username}
        />
        Password:
        <input
          type="password"
          name="Password"
          onChange={handleChange}
          value={user.Password}
        />
        <button>Submit</button>
      </form>
    </>
  );
}
