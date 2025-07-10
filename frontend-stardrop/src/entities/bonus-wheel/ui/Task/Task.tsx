import Image from 'next/image'

import classes from './Task.module.scss'

type TaskProps = {
  taskText: string
  isDone?: boolean
}

export const Task = ({ taskText, isDone }: TaskProps) => {
  return (
    <div className={classes.task}>
      <div className={classes.checkmark}>
        {isDone ? (
          <Image src='/icons/checkmark-green.svg' width={12} height={12} alt='Галочка' />
        ) : null}
      </div>

      <p>{taskText}</p>
    </div>
  )
}
