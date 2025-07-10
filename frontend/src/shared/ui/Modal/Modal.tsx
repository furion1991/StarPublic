import Popup from 'reactjs-popup'
import cn from 'classnames'
import 'reactjs-popup/dist/index.css'

import classes from './Modal.module.scss'
import './Modal.scss'

type ModalProps = {
  open: boolean
  children: React.ReactNode
  className?: string
  onClose: () => void
}

const Modal = ({ children, open, className = '', onClose }: ModalProps) => {
  return (
    <Popup className={classes.modalContainer} open={open} onClose={onClose} closeOnDocumentClick>
      <div className={cn(classes.modal, className)}>{children}</div>
    </Popup>
  )
}

const Header = ({ children }: { children: React.ReactNode }) => {
  return (
    <div className={classes.header}>
      <div className={classes.headerContent}>{children}</div>
    </div>
  )
}

const Content = ({ children }: { children: React.ReactNode }) => {
  return <div className={classes.content}>{children}</div>
}

Modal.Header = Header
Modal.Content = Content

export { Modal }
